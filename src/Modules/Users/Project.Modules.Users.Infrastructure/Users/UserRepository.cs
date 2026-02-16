
using Project.Modules.Users.Application.Abstractions.Users;
using Project.Modules.Users.Domain.Users;
using Project.Modules.Users.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Project.Modules.Users.Infrastructure.Users;

internal sealed class UserRepository(UsersDbContext dbContext) : IUserRepository
{
    public async Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await dbContext.Users
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
    }


    public async Task<User> AddAsync(User entity, CancellationToken cancellationToken = default)
    {
        EntityEntry<User> result = await dbContext.Users.AddAsync(entity, cancellationToken);
        return result.Entity;
    }

    public void Update(User entity, CancellationToken cancellationToken = default)
    {
        dbContext.Users.Update(entity);
    }

    public void Delete(User entity, CancellationToken cancellationToken = default)
    {
        dbContext.Users.Remove(entity);
    }


    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await dbContext.Users
            .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
    }
}
