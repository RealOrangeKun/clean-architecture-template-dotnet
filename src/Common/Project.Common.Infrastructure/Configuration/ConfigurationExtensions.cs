using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Project.Common.Infrastructure.Configuration;

public static class ConfigurationExtensions
{
    public static string GetConnectionStringOrThrow(this IConfiguration configuration, string name)
    {
        return configuration.GetConnectionString(name) ??
                throw new InvalidOperationException($"Connection string '{name}' not found in configuration.");
    }

    public static T GetValueOrThrow<T>(this IConfiguration configuration, string name)
    {
        return configuration.GetValue<T?>(name) ??
               throw new InvalidOperationException($"Configuration value '{name}' not found or is null.");
    }

    public static T GetConfigurationSectionOrThrow<T>(this IConfiguration configuration, string name)
    {
        return configuration.GetSection(name).Get<T>()
            ?? throw new InvalidOperationException($"Configuration section '{name}' not found or could not be bound to type '{typeof(T).Name}'.");
    }

    public static IConfigurationSection GetConfigurationSectionOrThrow(this IConfiguration configuration, string name)
    {
        return configuration.GetSection(name).Exists()
            ? configuration.GetSection(name)
            : throw new InvalidOperationException($"Configuration section '{name}' not found.");
    }

    public static InfrastructureOptions BuildInfrastructureOptions(
        this IConfiguration configuration,
        ILoggingBuilder loggingBuilder,
        Action<IRegistrationConfigurator>[]? moduleConfigureConsumers = null)
    {
        return new InfrastructureOptions
        {
            ConnectionStrings = new ConnectionStrings
            {
                Database = configuration.GetConnectionStringOrThrow(nameof(ConnectionStrings.Database)),
                Redis = configuration.GetConnectionStringOrThrow(nameof(ConnectionStrings.Redis))
            },
            Email = new EmailConfiguration
            {
                From = configuration.GetValueOrThrow<string>($"{EmailConfiguration.SectionName}:{nameof(EmailConfiguration.From)}"),
                Host = configuration.GetValueOrThrow<string>($"{EmailConfiguration.SectionName}:{nameof(EmailConfiguration.Host)}"),
                Port = configuration.GetValueOrThrow<int>($"{EmailConfiguration.SectionName}:{nameof(EmailConfiguration.Port)}")
            },
            OpenTelemetryOptions = new OpenTelemetryOptions
            {
                ServiceName = configuration.GetValueOrThrow<string>($"{OpenTelemetryOptions.SectionName}:{nameof(OpenTelemetryOptions.ServiceName)}"),
                Version = configuration.GetValueOrThrow<string>($"{OpenTelemetryOptions.SectionName}:{nameof(OpenTelemetryOptions.Version)}")
            },
            RabbitMq = configuration.GetConfigurationSectionOrThrow<RabbitMqConfiguration>(RabbitMqConfiguration.SectionName),
            LoggingBuilder = loggingBuilder,
            ModuleConfigureConsumers = moduleConfigureConsumers ?? []
        };
    }

}
