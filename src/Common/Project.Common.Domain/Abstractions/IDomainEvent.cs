using MediatR;

namespace Project.Common.Domain.Abstractions;

public interface IDomainEvent : INotification
{
    Guid Id { get; }
    DateTime OccurredOnUtc { get; }
}
