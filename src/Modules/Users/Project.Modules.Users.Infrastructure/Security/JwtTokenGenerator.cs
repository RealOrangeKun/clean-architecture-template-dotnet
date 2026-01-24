using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Project.Modules.Users.Application.Abstractions.Security;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Project.Modules.Users.Infrastructure.Security;

public class JwtTokenGenerator(IOptions<JwtSettings> jwtOptions) : IJwtTokenGenerator
{
    private readonly JwtSettings _jwtSettings = jwtOptions.Value;

    public string GenerateToken(Guid userId, string role)
    {
        string key = _jwtSettings.Key ?? throw new InvalidOperationException("JWT signing key not configured.");
        string? issuer = _jwtSettings.Authority;
        string? audience = _jwtSettings.Audience;
        int expiresInMinutes = _jwtSettings.ExpiresInMinutes > 0 ? _jwtSettings.ExpiresInMinutes : 60;
        DateTime expires = DateTime.UtcNow.AddMinutes(expiresInMinutes);

        Claim[] claims =
        [
            new(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new(ClaimTypes.Role, role)
        ];

        SymmetricSecurityKey securityKey = new(Encoding.UTF8.GetBytes(key));
        SigningCredentials credentials = new(securityKey, SecurityAlgorithms.HmacSha256);

        JwtSecurityToken token = new(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: expires,
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}


