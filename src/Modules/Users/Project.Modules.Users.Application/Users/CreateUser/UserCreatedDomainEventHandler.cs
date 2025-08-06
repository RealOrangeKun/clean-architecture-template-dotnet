using System.Data.Common;
using FluentResults;
using MediatR;
using Project.Common.Application.Data;
using Project.Common.Application.Email;
using Project.Common.Application.Messaging;
using Project.Modules.Users.Application.Users.GetUser;
using Project.Modules.Users.Domain.Users;

namespace Project.Modules.Users.Application.Users.CreateUser;

internal sealed class UserCreatedDomainEventHandler(
    IEmailService emailService,
    ISender sender)
    : DomainEventHandler<UserCreatedDomainEvent>
{
    public override async Task HandleAsync(UserCreatedDomainEvent domainEvent, CancellationToken cancellationToken = default)
    {
        Guid userId = domainEvent.UserId;

        Result<UserResponse> userResult = await sender.Send(new GetUserQuery(userId), cancellationToken);

        UserResponse user = userResult.Value;

        string templatePath = Path.Combine(AppContext.BaseDirectory, "Templates", "Emails", "WelcomeUser.cshtml");

        await emailService.SendTemplateAsync(
            user.Email,
            "Welcome to Project!",
            templatePath,
            new { user.FirstName },
            cancellationToken
        );
    }
}
