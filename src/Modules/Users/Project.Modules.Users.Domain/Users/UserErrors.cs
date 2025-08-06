using FluentResults;
using Project.Common.Domain;

namespace Project.Modules.Users.Domain.Users;

public static class UserErrors
{
    public static Error UserNotFound(Guid userId) =>
        new Error($"User with ID {userId} not found.")
            .WithErrorType(ErrorType.NotFound);

    public static Error CredentialsNotCorrect =>
        new Error("Invalid email or password.")
            .WithErrorType(ErrorType.Unauthorized);

    public static Error UserAlreadyExists =>
        new Error("User already exists")
            .WithErrorType(ErrorType.Conflict);
}
