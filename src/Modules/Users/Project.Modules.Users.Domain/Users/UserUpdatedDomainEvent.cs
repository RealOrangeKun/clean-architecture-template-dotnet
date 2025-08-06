using Project.Common.Domain.Abstractions;

namespace Project.Modules.Users.Domain.Users;

public sealed class UserUpdatedDomainEvent(Guid UserId, string FirstName, string LastName, string Email) : DomainEvent
{

    public Guid UserId { get; init; } = UserId;
    public string FirstName { get; init; } = FirstName;
    public string LastName { get; init; } = LastName;
    public string Email { get; init; } = Email;

}
