using Project.Common.Domain.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Project.Common.Infrastructure.Interceptors;

public class DomainEventsDispatcherInterceptor(
    ILogger<DomainEventsDispatcherInterceptor> logger,
    IServiceScopeFactory scopeFactory)
    : SaveChangesInterceptor
{

    public override async ValueTask<int> SavedChangesAsync(
        SaveChangesCompletedEventData eventData,
        int result,
        CancellationToken cancellationToken = default)
    {
        DbContext? dbContext = eventData.Context
            ?? throw new InvalidOperationException("DbContext cannot be null.");

        List<Entity> entitiesWithEvents = [.. dbContext
            .ChangeTracker
            .Entries<Entity>()
            .Select(e => e.Entity)
            .Where(e => e.DomainEvents.Count != 0)];

        var domainEvents = entitiesWithEvents
            .SelectMany(e => e.DomainEvents)
            .ToList();

        using IServiceScope scope = scopeFactory.CreateScope();
        IMediator mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        foreach (IDomainEvent? domainEvent in domainEvents)
        {
            if (domainEvent is not null)
            {
                logger.LogInformation("Publishing domain event: {EventType}", domainEvent.GetType().Name);
                await mediator.Publish(domainEvent, cancellationToken);
            }
        }

        foreach (Entity? entity in entitiesWithEvents)
        {
            entity.ClearDomainEvents();
        }

        return result;
    }
}
