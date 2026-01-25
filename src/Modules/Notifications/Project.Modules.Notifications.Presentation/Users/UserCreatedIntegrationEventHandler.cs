using FluentResults;
using MediatR;
using Project.Common.Application.EventBus;
using Project.Common.Application.Exceptions;
using Project.Modules.Notifications.Application.Emails.SendWelcomeEmail;
using Project.Modules.Users.IntegrationEvents.Users;

namespace Project.Modules.Notifications.Presentation.Users;

internal sealed class UserCreatedIntegrationEventHandler(
    ISender sender)
    : IntegrationEventHandler<UserCreatedIntegrationEvent>
{
    public async override Task Handle(UserCreatedIntegrationEvent integrationEvent, CancellationToken cancellationToken = default)
    {
        Result result = await sender.Send(new SendWelcomeEmailCommand(
            integrationEvent.Email,
            integrationEvent.FirstName,
            integrationEvent.LastName), cancellationToken);

        if (result.IsFailed)
        {
            throw new ProjectException(nameof(SendWelcomeEmailCommand), (Error)result.Errors);
        }
    }
}
