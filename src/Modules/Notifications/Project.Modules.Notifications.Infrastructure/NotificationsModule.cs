using Project.Modules.Notifications.Application.Abstractions.Emails;
using Project.Modules.Notifications.Infrastructure.Emails;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MassTransit;
using Project.Modules.Notifications.Infrastructure.Inbox;
using Project.Common.Presentation.Endpoints;
using Project.Common.Application.Messaging;
using Project.Common.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql;
using Project.Modules.Notifications.Infrastructure.Database;
using Project.Modules.Notifications.Application.Abstractions.Data;
using Project.Modules.Users.IntegrationEvents.Users;

namespace Project.Modules.Notifications.Infrastructure;

public static class NotificationsModule
{
    public static IServiceCollection AddNotificationsModule(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services
            .AddIntegrationEventHandlers(typeof(IdempotentIntegrationEventHandler<>), Presentation.AssemblyReference.Assembly)
            .AddModuleEndpoints(Presentation.AssemblyReference.Assembly);

        services.AddInfrastructure(configuration);

        return services;
    }

    private static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContextPool<NotificationsDbContext>((sp, options) =>
            options
                .UseNpgsql(
                    sp.GetRequiredService<NpgsqlDataSource>(),
                    npgsqlOptions => npgsqlOptions
                        .MigrationsHistoryTable(HistoryRepository.DefaultTableName, Schemas.Notifications))
                .UseSnakeCaseNamingConvention());

        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<NotificationsDbContext>());

        services.AddEmailServices(configuration);

        services.Configure<InboxOptions>(configuration.GetSection("Notifications:Inbox"));

        services.ConfigureOptions<ConfigureProcessInboxJob>();

        return services;
    }

    private static IServiceCollection AddEmailServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        EmailConfiguration emailConfig = configuration.GetSection("Email").Get<EmailConfiguration>()
            ?? throw new InvalidOperationException("Email configuration is missing");

        services.AddOptions<FluentEmailOptions>()
            .BindConfiguration(FluentEmailOptions.SectionName);

        services.AddFluentEmail(emailConfig.From)
            .AddRazorRenderer()
            .AddSmtpSender(emailConfig.Host, emailConfig.Port);

        services.TryAddScoped<IEmailService, EmailService>();

        return services;
    }

    public static void ConfigureConsumers(IRegistrationConfigurator registrationConfigurator)
    {
        registrationConfigurator.AddConsumer<IntegrationEventConsumer<UserCreatedIntegrationEvent>>();
    }
}
