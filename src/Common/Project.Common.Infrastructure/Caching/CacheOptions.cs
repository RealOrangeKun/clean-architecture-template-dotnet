
using Microsoft.Extensions.Caching.Distributed;

namespace Project.Common.Infrastructure.Caching;

public static class CacheOptions
{
    public static DistributedCacheEntryOptions DefaultExpiration => new()
    {
        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
    };

    public static DistributedCacheEntryOptions Create(TimeSpan? expiration) =>
        expiration is not null ?
            new() { AbsoluteExpirationRelativeToNow = expiration } :
            DefaultExpiration;
}
