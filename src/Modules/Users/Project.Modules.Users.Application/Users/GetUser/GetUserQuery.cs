using Project.Common.Application.Messaging;

namespace Project.Modules.Users.Application.Users.GetUser;

public sealed record GetUserQuery(int Id) : IQuery<UserResponse>;

