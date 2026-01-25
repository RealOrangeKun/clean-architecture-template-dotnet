using MassTransit;
using Microsoft.Extensions.Logging;

namespace Project.Common.Infrastructure.Configuration;

public sealed class InfrastructureOptions
{
    public ConnectionStrings ConnectionStrings { get; set; } = new();
    public RabbitMqConfiguration RabbitMq { get; set; } = new();
    public OpenTelemetryOptions OpenTelemetryOptions { get; set; } = new();
    public ILoggingBuilder LoggingBuilder { get; set; }
    public Action<IRegistrationConfigurator>[] ModuleConfigureConsumers { get; set; } = [];
}
