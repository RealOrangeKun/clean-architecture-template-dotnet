using Microsoft.Extensions.DependencyInjection;

namespace Project.Common.Infrastructure.Authorization;

internal static class AuthorizationExtensions
{
    extension(IServiceCollection services)
    {
        internal IServiceCollection AddAuthorizationInternal()
        {
            services.AddAuthorizationBuilder();

            return services;
        }

    }
}
