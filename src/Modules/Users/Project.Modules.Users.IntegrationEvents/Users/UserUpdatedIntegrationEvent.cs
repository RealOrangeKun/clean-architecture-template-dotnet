using Project.Common.Application.EventBus;

namespace Project.Modules.Users.IntegrationEvents.Users;

public sealed record UserUpdatedIntegrationEvent(
    Guid Id,
    DateTime OccurredOnUtc,
    Guid UserId,
    string Email,
    string FirstName,
    string LastName,
    string PhoneNumber,
    string Role
) : IntegrationEvent(Id, OccurredOnUtc);


