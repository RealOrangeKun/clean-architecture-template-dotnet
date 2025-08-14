using Project.Common.Infrastructure.Interceptors;
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
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Reflection;
using Project.Common.Infrastructure.Outbox;

namespace Project.Modules.Users.Infrastructure;

public static class UsersModule
{
    public static IServiceCollection AddUsersModule(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDomainEventHandlers();

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

        services.ConfigureOptions<ConfigureProcessOutboxJob>();

        return services;
    }

    private static void AddDomainEventHandlers(this IServiceCollection services)
    {
        services.Scan(scan => scan
            .FromAssemblies(Application.AssemblyReference.Assembly)
            .AddClasses(classes => classes.AssignableTo(typeof(IDomainEventHandler<>)), publicOnly: false)
            .AsImplementedInterfaces()
            .WithScopedLifetime());

        services.Decorate(typeof(IDomainEventHandler<>), typeof(IdempotentDomainEventHandler<>));
    }

}
