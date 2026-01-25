using Project.Common.Application.Caching;
using Project.Common.Application.Data;
using Project.Common.Infrastructure.Authentication;
using Project.Common.Infrastructure.Caching;
using Project.Common.Infrastructure.Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Quartz;
using Project.Common.Infrastructure.Outbox;
using MassTransit;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using OpenTelemetry.Metrics;
using OpenTelemetry.Logs;
using Project.Common.Application.EventBus;
using Project.Common.Infrastructure.Configuration;
using Project.Common.Infrastructure.Authorization;

namespace Project.Common.Infrastructure;

public static class InfrastructureConfiguration
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        InfrastructureOptions options)
    {
        services.AddAuthorizationInternal();

        services.AddAuthenticationInternal();

        services.AddNpgsqlDataSource(options.ConnectionStrings.Database);

        services.TryAddScoped<IDbConnectionFactory, DbConnectionFactory>();

        services.TryAddSingleton<InsertOutboxMessagesInterceptor>();

        services.AddQuartz();

        services.AddQuartzHostedService(options => options.WaitForJobsToComplete = true);

        services.AddCachingInternal(options.ConnectionStrings.Redis);

        services.TryAddSingleton<ICacheService, CacheService>();

        services.TryAddSingleton<IEventBus, EventBus.EventBus>();

        services.AddMassTransit(configure =>
        {
            foreach (Action<IRegistrationConfigurator> configureConsumers in options.ModuleConfigureConsumers)
            {
                configureConsumers(configure);
            }

            configure.SetKebabCaseEndpointNameFormatter();

            configure.UsingRabbitMq((ctx, cfg) =>
            {
                cfg.Host(options.RabbitMq.Host, options.RabbitMq.VirtualHost, h =>
                {
                    h.Username(options.RabbitMq.Username);
                    h.Password(options.RabbitMq.Password);
                });

                cfg.UseMessageRetry(r =>
                {
                    r.Exponential(
                        retryLimit: 5,
                        minInterval: TimeSpan.FromSeconds(5),
                        maxInterval: TimeSpan.FromMinutes(1),
                        intervalDelta: TimeSpan.FromSeconds(5)
                    );
                });

                cfg.UseScheduledRedelivery(r =>
                {
                    r.Intervals(
                        TimeSpan.FromMinutes(1),
                        TimeSpan.FromMinutes(5),
                        TimeSpan.FromMinutes(15),
                        TimeSpan.FromMinutes(30)
                    );
                });

                cfg.UseCircuitBreaker(cb =>
                {
                    cb.TrackingPeriod = TimeSpan.FromMinutes(1);
                    cb.TripThreshold = 15;
                    cb.ActiveThreshold = 10;
                    cb.ResetInterval = TimeSpan.FromMinutes(5);
                });

                cfg.ConfigureEndpoints(ctx);
            });
        });

        services.AddOpenTelemetryInternal(options.LoggingBuilder, options);

        return services;
    }

    private static IServiceCollection AddOpenTelemetryInternal(
        this IServiceCollection services,
        ILoggingBuilder logging,
        InfrastructureOptions options)
    {
        services.AddOpenTelemetry()
            .ConfigureResource(resource => resource
                .AddService(
                    options.OpenTelemetryOptions.ServiceName,
                    serviceVersion: options.OpenTelemetryOptions.Version)
                .AddAttributes(options.OpenTelemetryOptions.ServiceAttributes))
            .WithTracing(traceProvider => traceProvider
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation()
                .AddSource(MassTransit.Logging.DiagnosticHeaders.DefaultListenerName)
                .SetSampler(new TraceIdRatioBasedSampler(0.1))
                .AddConsoleExporter())
            .WithMetrics(meterProvider => meterProvider
                .AddAspNetCoreInstrumentation()
                .AddConsoleExporter());

        logging.ClearProviders();
        logging.AddOpenTelemetry(o =>
        {
            o.AddConsoleExporter();
            o.IncludeScopes = true;
            o.IncludeFormattedMessage = true;
        });

        return services;
    }
}