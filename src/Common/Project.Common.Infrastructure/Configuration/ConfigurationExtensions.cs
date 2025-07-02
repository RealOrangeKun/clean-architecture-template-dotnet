using Microsoft.Extensions.Configuration;

namespace Project.Common.Infrastructure.Configuration;

public static class ConfigurationExtensions
{
    public static string GetConnectionStringOrThrow(this IConfiguration configuration, string name)
    {
        return configuration.GetConnectionString(name) ??
                throw new InvalidOperationException($"Connection string '{name}' not found in configuration.");
    }

}
