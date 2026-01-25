using FluentResults;
using MediatR;
using Project.Common.Application.EventBus;
using Project.Common.Application.Exceptions;
using Project.Common.Application.Messaging;
using Project.Modules.Users.Application.Users.GetUser;
using Project.Modules.Users.Domain.Users;
using Project.Modules.Users.IntegrationEvents.Users;

namespace Project.Modules.Users.Application.Users.CreateUser;

internal sealed class UserCreatedDomainEventHandler(
    IEventBus eventBus,
    ISender sender)
    : DomainEventHandler<UserCreatedDomainEvent>
{
    public override async Task HandleAsync(UserCreatedDomainEvent domainEvent, CancellationToken cancellationToken = default)
    {
        Result<UserResponse> result = await sender.Send(new GetUserQuery(domainEvent.UserId), cancellationToken);

        if (result.IsFailed)
        {
            throw new ProjectException(nameof(GetUserQuery), (Error)result.Errors);
        }

        await eventBus.PublishAsync(new UserCreatedIntegrationEvent(
                domainEvent.Id,
                domainEvent.OccurredOnUtc,
                result.Value.Id,
                result.Value.Email,
                result.Value.FirstName,
                result.Value.LastName,
                result.Value.Role), cancellationToken);
    }
}
