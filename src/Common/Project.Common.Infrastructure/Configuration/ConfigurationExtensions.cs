using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Project.Common.Infrastructure.Configuration;

public static class ConfigurationExtensions
{
    extension(IConfiguration configuration)
    {
        public string GetConnectionStringOrThrow(string name) =>
            configuration.GetConnectionString(name)
                ?? throw new InvalidOperationException($"Connection string '{name}' not found in configuration.");

        public T GetValueOrThrow<T>(string name) =>
            configuration.GetValue<T?>(name)
                ?? throw new InvalidOperationException($"Configuration value '{name}' not found or is null.");

        public T GetConfigurationSectionOrThrow<T>(string name) =>
            configuration.GetSection(name).Get<T>()
                ?? throw new InvalidOperationException($"Configuration section '{name}' not found or could not be bound to type '{typeof(T).Name}'.");

        public IConfigurationSection GetConfigurationSectionOrThrow(string name) =>
            configuration.GetSection(name).Exists()
                ? configuration.GetSection(name)
                : throw new InvalidOperationException($"Configuration section '{name}' not found.");

        public InfrastructureOptions BuildInfrastructureOptions(
            ILoggingBuilder loggingBuilder,
            Action<IRegistrationConfigurator>[]? moduleConfigureConsumers = null) =>
            new()
            {
                ConnectionStrings = new()
                {
                    Database = configuration.GetConnectionStringOrThrow(nameof(ConnectionStrings.Database)),
                    Redis = configuration.GetConnectionStringOrThrow(nameof(ConnectionStrings.Redis))
                },
                OpenTelemetryOptions = new()
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
