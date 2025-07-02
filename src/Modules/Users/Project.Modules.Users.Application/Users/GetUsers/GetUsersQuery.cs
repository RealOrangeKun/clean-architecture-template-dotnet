
using Project.Common.Application.Messaging;

namespace Project.Modules.Users.Application.Users.GetUsers;

public sealed record GetUsersQuery : IQuery<IReadOnlyCollection<UserResponse>>;

