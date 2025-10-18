using Project.Common.Domain.Abstractions;

namespace Project.Modules.Users.Domain.Users;

public sealed record UserUpdatedDomainEvent(
    Guid Id,
    DateTime OccurredOnUtc,
    Guid UserId,
    string FirstName,
    string LastName,
    string Email,
    string Role) : DomainEvent(Id, OccurredOnUtc);