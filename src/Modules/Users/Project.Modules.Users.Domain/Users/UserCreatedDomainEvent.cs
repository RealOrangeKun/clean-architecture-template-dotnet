using Project.Common.Domain.Abstractions;

namespace Project.Modules.Users.Domain.Users;

public sealed record UserCreatedDomainEvent(
    Guid Id,
    DateTime OccurredOnUtc,
    Guid UserId) : DomainEvent(Id, OccurredOnUtc);
