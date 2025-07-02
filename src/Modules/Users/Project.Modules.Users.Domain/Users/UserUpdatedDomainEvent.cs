using Project.Common.Domain.Abstractions;

namespace Project.Modules.Users.Domain.Users;

public sealed class UserUpdatedDomainEvent(int UserId, string FirstName, string LastName, string Email) : DomainEvent
{

    public int UserId { get; init; } = UserId;
    public string FirstName { get; init; } = FirstName;
    public string LastName { get; init; } = LastName;
    public string Email { get; init; } = Email;

}
