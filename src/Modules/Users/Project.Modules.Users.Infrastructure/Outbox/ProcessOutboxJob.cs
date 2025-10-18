using System.Data;
using System.Data.Common;
using Dapper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Project.Common.Application.Data;
using Project.Common.Application.Messaging;
using Project.Common.Domain.Abstractions;
using Project.Common.Infrastructure.Outbox;
using Project.Common.Infrastructure.Serialization;
using Project.Modules.Users.Infrastructure.Database;
using Quartz;

namespace Project.Modules.Users.Infrastructure.Outbox;

[DisallowConcurrentExecution]
internal sealed class ProcessOutboxJob(
    IDbConnectionFactory dbConnectionFactory,
    IServiceScopeFactory serviceScopeFactory,
    IOptions<OutboxOptions> outboxOptions,
    ILogger<ProcessOutboxJob> logger)
    : IJob
{
    private const string ModuleName = "Users";

    public async Task Execute(IJobExecutionContext context)
    {
        logger.LogInformation("{Module} - Beginning to process outbox messages", ModuleName);

        await using DbConnection connection = await dbConnectionFactory.OpenConnectionAsync();
        await using DbTransaction transaction = await connection.BeginTransactionAsync();

        IReadOnlyList<OutboxMessageResponse> outboxMessages = await GetOutboxMessagesAsync(connection, transaction);

        var processedMessages = new List<ProcessedOutboxMessage>();

        foreach (OutboxMessageResponse outboxMessage in outboxMessages)
        {
            Exception? exception = null;
            try
            {
                IDomainEvent domainEvent = JsonConvert.DeserializeObject<IDomainEvent>(
                    outboxMessage.Content,
                    SerializerSettings.Instance)!;

                using IServiceScope scope = serviceScopeFactory.CreateScope();

                IEnumerable<IDomainEventHandler> domainEventHandlers = DomainEventHandlersFactory.GetHandlers(
                    domainEvent.GetType(),
                    scope.ServiceProvider,
                    Application.AssemblyReference.Assembly);

                foreach (IDomainEventHandler domainEventHandler in domainEventHandlers)
                {
                    await domainEventHandler.HandleAsync(domainEvent);
                }
            }
            catch (Exception caughtException)
            {
                logger.LogError(
                    caughtException,
                    "{Module} - Exception while processing outbox message {MessageId}",
                    ModuleName,
                    outboxMessage.Id);

                exception = caughtException;
            }

            processedMessages.Add(new ProcessedOutboxMessage(
                outboxMessage.Id,
                DateTime.UtcNow,
                exception?.ToString()));
        }

        if (processedMessages.Count > 0)
        {
            await UpdateOutboxMessagesAsync(connection, transaction, processedMessages);
        }

        await transaction.CommitAsync();

        logger.LogInformation("{Module} - Completed processing outbox messages", ModuleName);
    }

    private async Task<IReadOnlyList<OutboxMessageResponse>> GetOutboxMessagesAsync(
        IDbConnection connection,
        IDbTransaction transaction)
    {
        string sql =
            $"""
             SELECT
                id AS {nameof(OutboxMessageResponse.Id)},
                content AS {nameof(OutboxMessageResponse.Content)}
             FROM {Schemas.Users}.outbox_messages
             WHERE processed_on_utc IS NULL
             ORDER BY occurred_on_utc
             LIMIT {outboxOptions.Value.BatchSize}
             FOR UPDATE
             """;

        IEnumerable<OutboxMessageResponse> outboxMessages = await connection.QueryAsync<OutboxMessageResponse>(
            sql,
            transaction: transaction);

        return [.. outboxMessages];
    }

    private static async Task UpdateOutboxMessagesAsync(
        IDbConnection connection,
        IDbTransaction transaction,
        IReadOnlyList<ProcessedOutboxMessage> processedMessages)
    {
        if (processedMessages.Count == 0) return;

        var parameters = new DynamicParameters();
        var valuesClauses = new List<string>();

        for (int i = 0; i < processedMessages.Count; i++)
        {
            ProcessedOutboxMessage message = processedMessages[i];
            string idParam = $"Id{i}";
            string processedParam = $"ProcessedOn{i}";
            string errorParam = $"Error{i}";

            parameters.Add(idParam, message.Id);
            parameters.Add(processedParam, message.ProcessedOnUtc);
            parameters.Add(errorParam, message.Error);

            valuesClauses.Add($"(@{idParam}, @{processedParam}, @{errorParam})");
        }

        string sql = $"""
            UPDATE {Schemas.Users}.outbox_messages
            SET processed_on_utc = v.processed_on_utc,
                error = v.error
            FROM (VALUES
                {string.Join(",\n                ", valuesClauses)}
            ) AS v(id, processed_on_utc, error)
            WHERE outbox_messages.id = v.id::uuid
            """;

        await connection.ExecuteAsync(sql, parameters, transaction: transaction);
    }

    internal sealed record OutboxMessageResponse(Guid Id, string Content);

    internal sealed record ProcessedOutboxMessage(Guid Id, DateTime ProcessedOnUtc, string? Error);

}
