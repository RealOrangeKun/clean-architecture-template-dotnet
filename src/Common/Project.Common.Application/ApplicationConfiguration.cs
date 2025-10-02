using System.Reflection;
using FluentValidation;
using Project.Common.Application.Behaviors;
using Microsoft.Extensions.DependencyInjection;

namespace Project.Common.Application;

public static class ApplicationConfiguration
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services,
        Assembly[] moduleAssemblies)
    {

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssemblies(moduleAssemblies);

            cfg.AddOpenBehavior(typeof(ValidationPipelineBehavior<,>));
            cfg.AddOpenBehavior(typeof(LoggingPipelineBehavior<,>));
        });

        services.AddValidatorsFromAssemblies(moduleAssemblies, includeInternalTypes: true);

        return services;
    }
}
