using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Project.Common.Application.EventBus;
using Project.Common.Application.Messaging;
using Project.Common.Presentation.Endpoints;

namespace Project.Common.Infrastructure;

public static class CommonModule
{
    public static IServiceCollection AddDomainEventHandlers(
        this IServiceCollection services,
        Type idempotentDomainEventHandlerType,
        Assembly assembly)
    {
        services.Scan(scan => scan
            .FromAssemblies(assembly)
            .AddClasses(c => c.AssignableTo(typeof(IDomainEventHandler<>)), publicOnly: false)
            .AsImplementedInterfaces()
            .AsSelf()
            .WithScopedLifetime());

        services.TryDecorate(typeof(IDomainEventHandler<>), idempotentDomainEventHandlerType);

        return services;
    }

    public static IServiceCollection AddIntegrationEventHandlers(
        this IServiceCollection services,
        Type idempotentIntegrationEventHandlerType,
        Assembly assembly)
    {
        services.Scan(scan => scan
            .FromAssemblies(assembly)
            .AddClasses(c => c.AssignableTo(typeof(IIntegrationEventHandler<>)), publicOnly: false)
            .AsImplementedInterfaces()
            .AsSelf()
            .WithScopedLifetime());

        services.TryDecorate(typeof(IIntegrationEventHandler<>), idempotentIntegrationEventHandlerType);

        return services;
    }

    public static IServiceCollection AddModuleEndpoints(
        this IServiceCollection services,
        Assembly assembly)
    {
        services.AddEndpoints(assembly);
        return services;
    }
}
