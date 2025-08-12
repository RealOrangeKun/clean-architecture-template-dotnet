using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Project.Common.Infrastructure.Caching;

public sealed class HybridCacheConfigureOptions(IConfiguration configuration)
    : IConfigureNamedOptions<HybridCacheOptions>
{
    private const string _configurationSectionName = "HybridCache";

    public void Configure(string? name, HybridCacheOptions options)
    {
        Configure(options);
    }

    public void Configure(HybridCacheOptions options)
    {
        configuration.GetSection(_configurationSectionName).Bind(options);
    }
}
