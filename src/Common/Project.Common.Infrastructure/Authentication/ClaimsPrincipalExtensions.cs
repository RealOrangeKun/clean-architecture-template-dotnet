using System.Security.Claims;
using Project.Common.Application.Exceptions;

namespace Project.Common.Infrastructure.Authentication;

public static class ClaimsPrincipalExtensions
{

    public static int GetUserId(this ClaimsPrincipal? principal)
    {
        string? userId = principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        return int.TryParse(userId, out int parsedUserId) ?
            parsedUserId :
            throw new ProjectException("User ID not found in claims.");
    }

    public static string GetUserRole(this ClaimsPrincipal? principal)
    {
        return principal?.FindFirst(ClaimTypes.Role)?.Value ??
            throw new ProjectException("User role not found in claims.");
    }

}
