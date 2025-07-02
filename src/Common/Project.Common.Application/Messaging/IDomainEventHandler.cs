using Project.Common.Domain.Abstractions;
using MediatR;

namespace Project.Common.Application.Messaging;

public interface IDomainEventHandler<in TDomainEvent> : IDomainEventHandler, INotificationHandler<TDomainEvent>
    where TDomainEvent : IDomainEvent;

public interface IDomainEventHandler;
