namespace Project.Modules.Users.Application.Users.GetUsers;

public sealed record UserResponse(Guid Id, string FirstName, string LastName, string Email);
