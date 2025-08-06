using Project.Common.Application.Messaging;

namespace Project.Modules.Users.Application.Users.GetUser;

public sealed record GetUserQuery(Guid Id) : IQuery<UserResponse>;

