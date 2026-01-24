using Project.Common.Presentation.Endpoints;
using Project.Modules.Users.Application.Abstractions.Data;
using Project.Modules.Users.Application.Abstractions.Security;
using Project.Modules.Users.Application.Abstractions.Users;
using Project.Modules.Users.Infrastructure.Database;
using Project.Modules.Users.Infrastructure.Security;
using Project.Modules.Users.Infrastructure.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using Project.Modules.Users.Infrastructure.Outbox;
using Project.Common.Application.Messaging;
using Project.Common.Infrastructure.Outbox;
using Project.Common.Application.EventBus;
using Project.Modules.Users.Infrastructure.Inbox;
using MassTransit;
using Project.Common.Infrastructure;

namespace Project.Modules.Users.Infrastructure;

public static class UsersModule

{
    public static IServiceCollection AddUsersModule(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services
            .AddDomainEventHandlers(typeof(IdempotentDomainEventHandler<>), Application.AssemblyReference.Assembly)
            .AddIntegrationEventHandlers(typeof(IdempotentIntegrationEventHandler<>), Presentation.AssemblyReference.Assembly)
            .AddModuleEndpoints(Presentation.AssemblyReference.Assembly);

        services.AddInfrastructure(configuration);

        return services;
    }
    private static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddScoped<IPasswordHasher, PasswordHasher>();

        services.AddDbContextPool<UsersDbContext>((sp, options) =>
            options
                .UseNpgsql(
                    sp.GetRequiredService<NpgsqlDataSource>(),
                    npgsqlOptions => npgsqlOptions
                        .MigrationsHistoryTable(HistoryRepository.DefaultTableName, Schemas.Users))
                .AddInterceptors(sp.GetRequiredService<InsertOutboxMessagesInterceptor>())
                .UseSnakeCaseNamingConvention());

        services.AddScoped<IUserRepository, UserRepository>();

        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<UsersDbContext>());

        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();

        services.Configure<JwtSettings>(
            configuration.GetSection("Authentication"));

        services.Configure<OutboxOptions>(configuration.GetSection("Users:Outbox"));

        services.Configure<InboxOptions>(configuration.GetSection("Users:Inbox"));

        services.ConfigureOptions<ConfigureProcessOutboxJob>();

        services.ConfigureOptions<ConfigureProcessInboxJob>();

        return services;
    }

    /// <summary>
    /// Registers the <see cref="IntegrationEventConsumer{T}"/> for specific User module events.
    /// </summary>
    /// <param name="registrationConfigurator">The MassTransit configurator.</param>
    /// <remarks>
    /// This ensures the module listens for integration events via the configured message broker.
    /// </remarks>
    /// <example>
    /// <code>
    /// registrationConfigurator.AddConsumer&lt;IntegrationEventConsumer&lt;UserCreatedIntegrationEvent&gt;&gt;();
    /// </code>
    /// </example>
    public static void ConfigureConsumers(IRegistrationConfigurator registrationConfigurator)
    {
        // registrationConfigurator.AddConsumer<IntegrationEventConsumer<UserCreatedIntegrationEvent>>();
    }

}
