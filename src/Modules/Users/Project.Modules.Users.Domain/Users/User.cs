
using Project.Common.Domain.Abstractions;

namespace Project.Modules.Users.Domain.Users;

public sealed class User : Entity
{
    private User() { }
    public int Id { get; private set; }
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public string Email { get; private set; }
    public string HashedPassword { get; private set; }
    public Role Role { get; private set; } = Role.User;

    public static User Create(string firstName, string lastName, string email, string hashedPassword)
    {
        var user = new User
        {
            FirstName = firstName,
            LastName = lastName,
            Email = email,
            HashedPassword = hashedPassword
        };
        user.Raise(new UserCreatedDomainEvent(user));
        return user;
    }

    public void Update(string firstName, string lastName, string email)
    {
        FirstName = firstName;
        LastName = lastName;
        Email = email;

        Raise(new UserUpdatedDomainEvent(Id, firstName, lastName, email));
    }

}
