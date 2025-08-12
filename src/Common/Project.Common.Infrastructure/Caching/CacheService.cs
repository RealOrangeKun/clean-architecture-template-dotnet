using Microsoft.Extensions.Caching.Hybrid;
using Project.Common.Application.Caching;

namespace Project.Common.Infrastructure.Caching;

internal sealed class CacheService(HybridCache cache) : ICacheService
{
    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        return await cache.GetOrCreateAsync(
            key,
            factory: _ => ValueTask.FromResult<T?>(default),
            cancellationToken: cancellationToken);
    }

    public async Task<T> GetOrCreateAsync<T>(
        string key,
        Func<CancellationToken, Task<T>> factory,
        TimeSpan? expiration = null,
        CancellationToken cancellationToken = default)
    {
        HybridCacheEntryOptions? options = expiration.HasValue
            ? new HybridCacheEntryOptions { Expiration = expiration.Value }
            : null;

        return await cache.GetOrCreateAsync(
            key,
            async cancellationToken => await factory(cancellationToken),
            options,
            cancellationToken: cancellationToken);
    }

    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        await cache.RemoveAsync(key, cancellationToken);
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken cancellationToken = default)
    {
        HybridCacheEntryOptions? options = expiration.HasValue
            ? new HybridCacheEntryOptions { Expiration = expiration.Value }
            : null;

        await cache.SetAsync(key, value, options, tags: null, cancellationToken);
    }
}
