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

namespace Project.Modules.Users.Infrastructure;

public static class UsersModule
{
    public static IServiceCollection AddUsersModule(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDomainEventHandlers();

        services.AddIntegrationEventHandlers();

        services.AddInfrastructure(configuration);

        services.AddEndpoints(Presentation.AssemblyReference.Assembly);

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

        services.Configure<OutboxOptions>(configuration.GetSection("Users:Inbox"));

        services.ConfigureOptions<ConfigureProcessOutboxJob>();

        services.ConfigureOptions<ConfigureProcessInboxJob>();

        return services;
    }

    private static void AddIntegrationEventHandlers(this IServiceCollection services)
    {
        services.Scan(scan => scan
            .FromAssemblies(Presentation.AssemblyReference.Assembly)
            .AddClasses(classes => classes.AssignableTo(typeof(IIntegrationEventHandler<>)), publicOnly: false)
            .AsImplementedInterfaces()
            .WithScopedLifetime());

        services.TryDecorate(typeof(IIntegrationEventHandler<>), typeof(IdempotentIntegrationEventHandler<>));
    }

    private static void AddDomainEventHandlers(this IServiceCollection services)
    {
        services.Scan(scan => scan
            .FromAssemblies(Application.AssemblyReference.Assembly)
            .AddClasses(classes => classes.AssignableTo(typeof(IDomainEventHandler<>)), publicOnly: false)
            .AsImplementedInterfaces()
            .WithScopedLifetime());

        services.TryDecorate(typeof(IDomainEventHandler<>), typeof(IdempotentDomainEventHandler<>));
    }

    public static void ConfigureConsumers(IRegistrationConfigurator registrationConfigurator)
    {
    }

}
