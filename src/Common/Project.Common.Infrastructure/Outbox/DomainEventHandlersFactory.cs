using System.Collections.Concurrent;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Project.Common.Application.Messaging;

namespace Project.Common.Infrastructure.Outbox;

public static class DomainEventHandlersFactory
{
    private static readonly ConcurrentDictionary<string, Type[]> HandlersDictionary = new();

    public static IEnumerable<IDomainEventHandler> GetHandlers(
        Type eventType,
        IServiceProvider serviceProvider,
        Assembly assembly)
    {
        Type handlerInterfaceType = typeof(IDomainEventHandler<>).MakeGenericType(eventType);

        Type[] handlerTypes = HandlersDictionary.GetOrAdd(
            $"{assembly.GetName().Name}:{eventType.FullName}",
            _ => [.. assembly
            .DefinedTypes
            .Where(t => handlerInterfaceType.IsAssignableFrom(t) && !t.IsAbstract && !t.IsInterface)
            .Select(t => t.AsType())]
        );

        foreach (Type handlertype in handlerTypes)
        {
            yield return (IDomainEventHandler)serviceProvider.GetRequiredService(handlertype);
        }


    }
}
