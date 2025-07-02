
namespace Project.Modules.Users.Application.Abstractions.Security;

public interface IJwtTokenGenerator
{
    string GenerateToken(int userId, string role);
}
