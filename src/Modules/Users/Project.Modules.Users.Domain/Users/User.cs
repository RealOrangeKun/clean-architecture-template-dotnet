
using Project.Common.Domain.Abstractions;

namespace Project.Modules.Users.Domain.Users;

public sealed class User : Entity
{
    private User() { }
    public Guid Id { get; private set; }
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public string Email { get; private set; }
    public string HashedPassword { get; private set; }
    public Role Role { get; private set; }

    public static User Create(string firstName, string lastName, string email, string hashedPassword)
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            FirstName = firstName,
            LastName = lastName,
            Email = email,
            HashedPassword = hashedPassword,
            Role = Role.User
        };
        user.Raise(new UserCreatedDomainEvent(Guid.NewGuid(), DateTime.UtcNow, user.Id));
        return user;
    }

    public void Update(string firstName, string lastName, string email)
    {
        FirstName = firstName;
        LastName = lastName;
        Email = email;

        Raise(new UserUpdatedDomainEvent(Guid.NewGuid(), DateTime.UtcNow, Id, firstName, lastName, email, Role.ToString()));
    }

}
