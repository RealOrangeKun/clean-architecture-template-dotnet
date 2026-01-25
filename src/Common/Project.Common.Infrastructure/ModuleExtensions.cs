using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Project.Common.Application.EventBus;
using Project.Common.Application.Messaging;
using Project.Common.Presentation.Endpoints;

namespace Project.Common.Infrastructure;

public static class ModuleExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddDomainEventHandlers(
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
        public IServiceCollection AddIntegrationEventHandlers(
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

        public IServiceCollection AddModuleEndpoints(
            Assembly assembly)
        {
            services.AddEndpoints(assembly);
            return services;
        }

    }

}
