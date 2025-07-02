using Project.Common.Domain.Abstractions;

namespace Project.Modules.Users.Domain.Users;

public sealed class UserCreatedDomainEvent(User user) : DomainEvent
{
    public User User { get; } = user;
}
