
namespace Project.Modules.Users.Application.Abstractions.Security;

public sealed class JwtSettings
{
    public string? Authority { get; set; }
    public string? Audience { get; set; }
    public string? Key { get; set; }
    public int ExpiresInMinutes { get; set; } = 60;
}
