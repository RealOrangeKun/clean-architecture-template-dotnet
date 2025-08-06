using Project.Common.Domain.Abstractions;

namespace Project.Modules.Users.Domain.Users;

public sealed class UserCreatedDomainEvent(Guid id) : DomainEvent
{
    public Guid UserId { get; } = id;
}
