using Project.Common.Application.Messaging;

namespace Project.Modules.Notifications.Application.Emails.SendWelcomeEmail;

public sealed record SendWelcomeEmailCommand(
    string Email,
    string FirstName,
    string LastName)
    : ICommand;
