
using Project.Modules.Users.Application.Abstractions.Data;
using Project.Modules.Users.Domain.Users;
using Project.Modules.Users.Infrastructure.Users;
using Microsoft.EntityFrameworkCore;
using PRoject.Common.Infrastructure.Outbox;
using Project.Common.Infrastructure.Outbox;

namespace Project.Modules.Users.Infrastructure.Database;

public class UsersDbContext(DbContextOptions<UsersDbContext> options)
    : DbContext(options), IUnitOfWork
{
    internal DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(Schemas.Users);
        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new OutboxMessageConfiguration());
        modelBuilder.ApplyConfiguration(new OutboxMessageConsumerConfiguration());
    }
}
