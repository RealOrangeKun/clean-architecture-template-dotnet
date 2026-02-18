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

        List<ProcessedOutboxMessage> processedMessages = [];

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

        string updateSql =
            $$"""
            UPDATE {{Schemas.Users}}.outbox_messages
            SET processed_on_utc = v.processed_on_utc,
                error = v.error
            FROM (VALUES
                {0}
            ) AS v(id, processed_on_utc, error)
            WHERE outbox_messages.id = v.id::uuid
            """;

        string paramNames = string.Join(",", processedMessages.Select((_, i) => $"(@Id{i}, @ProcessedOn{i}, @Error{i})"));
        string formattedSql = string.Format(updateSql, paramNames);

        DynamicParameters parameters = new();

        for (int i = 0; i < processedMessages.Count; i++)
        {
            ProcessedOutboxMessage message = processedMessages[i];
            parameters.Add($"Id{i}", message.Id);
            parameters.Add($"ProcessedOn{i}", message.ProcessedOnUtc);
            parameters.Add($"Error{i}", message.Error);
        }

        await connection.ExecuteAsync(formattedSql, parameters, transaction: transaction);
    }

    internal sealed record OutboxMessageResponse(Guid Id, string Content);

    internal sealed record ProcessedOutboxMessage(Guid Id, DateTime ProcessedOnUtc, string? Error);

}
