
namespace Project.Modules.Users.Application.Users.GetUser;

public sealed record UserResponse(int Id, string FirstName, string LastName, string Email, string Role, IReadOnlyCollection<string> Warnings, int TotalWarnings);
