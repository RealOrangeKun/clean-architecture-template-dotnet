using Project.Common.Domain.Abstractions;

namespace Project.Modules.Users.Domain.Users;

public sealed class UserCreatedDomainEvent(Guid userId) : DomainEvent
{
    public Guid UserId { get; } = userId;
}
