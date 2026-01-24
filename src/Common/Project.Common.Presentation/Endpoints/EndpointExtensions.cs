using System.Collections.Concurrent;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Project.Common.Presentation.Endpoints;

public static class EndpointExtensions
{
    private static readonly ConcurrentDictionary<Assembly, Type[]> _endpointCache = [];

    extension(IServiceCollection services)
    {
        public IServiceCollection AddEndpoints(Assembly assembly)
        {
            Type[] endpointTypes = _endpointCache.GetOrAdd(assembly, asm =>
                [.. asm.DefinedTypes
                        .Where(t => !t.IsAbstract && !t.IsInterface && t.IsAssignableTo(typeof(IEndpoint)))
                        .Select(t => t.AsType())]
            );

            foreach (Type type in endpointTypes)
            {
                services.TryAddEnumerable(ServiceDescriptor.Singleton(typeof(IEndpoint), type));
            }

            return services;
        }
    }

    extension(WebApplication app)
    {
        public IApplicationBuilder MapEndpoints(RouteGroupBuilder? routeGroupBuilder = null)
        {
            IEnumerable<IEndpoint> endpoints = app.Services
                .GetServices<IEndpoint>();

            IEndpointRouteBuilder builder = routeGroupBuilder is null ?
                app :
                routeGroupBuilder;

            foreach (IEndpoint endpoint in endpoints)
            {
                endpoint.MapEndpoint(builder);
            }

            return app;
        }
    }
}
