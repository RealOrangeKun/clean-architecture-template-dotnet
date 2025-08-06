using System.Security.Claims;
using Project.Common.Application.Exceptions;

namespace Project.Common.Infrastructure.Authentication;

public static class ClaimsPrincipalExtensions
{

    public static Guid GetUserId(this ClaimsPrincipal? principal)
    {
        string? userId = principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        return Guid.TryParse(userId, out Guid parsedUserId) ?
            parsedUserId :
            throw new ProjectException("User ID not found in claims.");
    }

    public static string GetUserRole(this ClaimsPrincipal? principal)
    {
        return principal?.FindFirst(ClaimTypes.Role)?.Value ??
            throw new ProjectException("User role not found in claims.");
    }

}
