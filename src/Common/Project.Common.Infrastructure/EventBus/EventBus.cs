using Project.Common.Application.EventBus;
using MassTransit;

namespace Project.Common.Infrastructure.EventBus;

internal sealed class EventBus(IBus bus) : IEventBus
{
    public async Task PublishAsync<T>(T message, CancellationToken cancellationToken = default)
        where T : IIntegrationEvent
    {
        await bus.Publish(message, cancellationToken);
    }
}

