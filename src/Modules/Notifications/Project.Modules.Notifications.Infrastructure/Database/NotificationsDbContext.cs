using Project.Modules.Notifications.Application.Abstractions.Data;
using Microsoft.EntityFrameworkCore;
using Project.Common.Infrastructure.Inbox;

namespace Project.Modules.Notifications.Infrastructure.Database;

public class NotificationsDbContext(DbContextOptions<NotificationsDbContext> options)
    : DbContext(options), IUnitOfWork
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(Schemas.Notifications);

        modelBuilder.ApplyConfiguration(new InboxMessageConfiguration());
        modelBuilder.ApplyConfiguration(new InboxMessageConsumerConfiguration());
    }
}
