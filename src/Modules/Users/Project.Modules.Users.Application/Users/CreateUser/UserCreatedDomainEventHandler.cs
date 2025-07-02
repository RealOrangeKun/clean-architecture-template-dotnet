using Project.Common.Application.Email;
using Project.Common.Application.Messaging;
using Project.Modules.Users.Domain.Users;

namespace Project.Modules.Users.Application.Users.CreateUser;

internal sealed class UserCreatedDomainEventHandler(
    IEmailService emailService)
    : DomainEventHandler<UserCreatedDomainEvent>
{
    public override async Task HandleAsync(UserCreatedDomainEvent domainEvent, CancellationToken cancellationToken = default)
    {
        User user = domainEvent.User;
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
