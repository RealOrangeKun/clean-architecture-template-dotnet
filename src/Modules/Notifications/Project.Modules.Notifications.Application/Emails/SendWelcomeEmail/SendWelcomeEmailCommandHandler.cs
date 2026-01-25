using FluentResults;
using Project.Common.Application.Messaging;
using Project.Modules.Notifications.Application.Abstractions.Emails;
using Project.Modules.Notifications.Application.Emails.Templates.Welcome;

namespace Project.Modules.Notifications.Application.Emails.SendWelcomeEmail;

internal sealed class SendWelcomeEmailCommandHandler(
    IEmailService emailService)
    : ICommandHandler<SendWelcomeEmailCommand>
{
    public async Task<Result> Handle(SendWelcomeEmailCommand request, CancellationToken cancellationToken)
    {
        var welcomeEmailTemplate = new WelcomeEmailTemplate(
            recipientEmail: request.Email,
            firstName: request.FirstName,
            lastName: request.LastName,
            loginUrl: null
        );

        bool success = await emailService.SendTemplateAsync(
            welcomeEmailTemplate,
            cancellationToken);

        return success
            ? Result.Ok()
            : Result.Fail("Failed to send welcome email");
    }
}