using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Project.Common.Application.Caching;

namespace Project.Common.Infrastructure.Caching;

public static class CacheExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddCachingInternal(string redisConnectionString)
        {
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = redisConnectionString;
            });

            services.ConfigureOptions<HybridCacheConfigureOptions>();

            services.AddHybridCache();

            services.TryAddScoped<ICacheService, CacheService>();

            return services;
        }
    }
}
