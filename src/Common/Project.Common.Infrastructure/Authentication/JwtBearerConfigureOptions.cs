using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Project.Common.Infrastructure.Authentication;

internal sealed class JwtBearerConfigureOptions(IConfiguration configuration)
    : IConfigureNamedOptions<JwtBearerOptions>
{
    private const string ConfigurationSectionName = "Authentication";
    public void Configure(string? name, JwtBearerOptions options)
    {
        Configure(options);
    }

    public void Configure(JwtBearerOptions options)
    {
        configuration.GetSection(ConfigurationSectionName).Bind(options);

        string? key = configuration.GetSection($"{ConfigurationSectionName}:Key").Get<string>();
        if (!string.IsNullOrWhiteSpace(key))
        {
            options.TokenValidationParameters.IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        }
        string? issuer = configuration[$"{ConfigurationSectionName}:Authority"];
        if (!string.IsNullOrWhiteSpace(issuer))
        {
            options.TokenValidationParameters.ValidIssuer = issuer;
        }
        string? audience = configuration[$"{ConfigurationSectionName}:Audience"];
        if (!string.IsNullOrWhiteSpace(audience))
        {
            options.TokenValidationParameters.ValidAudience = audience;
        }
    }
}
