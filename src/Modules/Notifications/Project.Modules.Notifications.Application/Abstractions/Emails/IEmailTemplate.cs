namespace Project.Modules.Notifications.Application.Abstractions.Emails;

public interface IEmailTemplate
{
    string TemplatePath { get; }
    string Subject { get; }
    string RecipientEmail { get; }
}
