using System.Collections.Concurrent;
using System.Reflection;
using Project.Common.Application.EventBus;
using Microsoft.Extensions.DependencyInjection;

namespace Project.Common.Infrastructure.Inbox;

public static class IntegrationEventHandlersFactory
{
    private static readonly ConcurrentDictionary<string, Type[]> HandlersDictionary = new();

    public static IEnumerable<IIntegrationEventHandler> GetHandlers(
        Type eventType,
        IServiceProvider serviceProvider,
        Assembly assembly)
    {
        Type handlerInterfaceType = typeof(IIntegrationEventHandler<>).MakeGenericType(eventType);

        Type[] handlerTypes = HandlersDictionary.GetOrAdd(
            $"{assembly.GetName().Name}:{eventType.FullName}",
            _ => [.. assembly
            .DefinedTypes
            .Where(t => handlerInterfaceType.IsAssignableFrom(t) && !t.IsAbstract && !t.IsInterface)
            .Select(t => t.AsType())]
        );

        foreach (Type handlerType in handlerTypes)
        {
            yield return (IIntegrationEventHandler)serviceProvider.GetRequiredService(handlerType);
        }


    }
}

