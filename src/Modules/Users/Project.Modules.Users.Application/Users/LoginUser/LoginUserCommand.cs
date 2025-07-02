using Project.Common.Application.Messaging;

namespace Project.Modules.Users.Application.Users.LoginUser;

public sealed record LoginUserCommand(string Email, string Password) : ICommand<LoginUserResponse>;
