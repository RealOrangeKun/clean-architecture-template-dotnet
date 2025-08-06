
namespace Project.Modules.Users.Application.Abstractions.Security;

public interface IJwtTokenGenerator
{
    string GenerateToken(Guid userId, string role);
}
