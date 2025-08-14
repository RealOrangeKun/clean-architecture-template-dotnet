using System.Collections.Concurrent;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Project.Common.Presentation.Endpoints;

public static class Endpointxtensions
{
    private static readonly ConcurrentDictionary<Assembly, Type[]> _endpointCache = [];
    public static IServiceCollection AddEndpoints(this IServiceCollection services,
        params Assembly[] assemblies)
    {
        foreach (Assembly assembly in assemblies)
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
        }

        return services;
    }

    public static IApplicationBuilder MapEndpoints(this WebApplication app,
        RouteGroupBuilder? routeGroupBuilder = null)
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
