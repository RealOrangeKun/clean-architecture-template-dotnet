
using Project.Common.Domain.Abstractions;
using Project.Modules.Users.Domain.Users;

namespace Project.Modules.Users.Application.Abstractions.Users;

public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
}
