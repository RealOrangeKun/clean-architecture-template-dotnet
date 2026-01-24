using Microsoft.Extensions.DependencyInjection;

namespace Project.Common.Infrastructure.Authentication;

internal static class AuthenticationExtensions
{
    extension(IServiceCollection services)
    {
        internal IServiceCollection AddAuthenticationInternal()
        {
            services.AddAuthentication().AddJwtBearer();

            services.AddHttpContextAccessor();

            services.ConfigureOptions<JwtBearerConfigureOptions>();

            return services;
        }
    }
}
