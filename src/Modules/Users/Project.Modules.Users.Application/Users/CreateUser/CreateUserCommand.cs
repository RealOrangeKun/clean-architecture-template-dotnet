using Project.Common.Application.Messaging;

namespace Project.Modules.Users.Application.Users.CreateUser;

public sealed record CreateUserCommand(
    string FirstName,
    string LastName,
    string Email,
    string Password) : ICommand<Guid>;
