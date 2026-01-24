using Project.Common.Application.EventBus;

namespace Project.Modules.Users.IntegrationEvents.Users;

public sealed record UserCreatedIntegrationEvent(
    Guid Id,
    DateTime OccurredOnUtc,
    Guid UserId,
    string Email,
    string FirstName,
    string LastName,
    string Role
) : IntegrationEvent(Id, OccurredOnUtc);

