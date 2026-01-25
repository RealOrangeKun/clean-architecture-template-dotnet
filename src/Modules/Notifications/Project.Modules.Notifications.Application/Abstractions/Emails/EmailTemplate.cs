namespace Project.Modules.Notifications.Application.Abstractions.Emails;

public abstract class EmailTemplate : IEmailTemplate
{
    protected EmailTemplate(string recipientEmail)
    {
        if (string.IsNullOrWhiteSpace(recipientEmail))
            throw new ArgumentException("Recipient email cannot be null or empty.", nameof(recipientEmail));

        RecipientEmail = recipientEmail;
    }

    public abstract string TemplatePath { get; }
    public abstract string Subject { get; }
    public string RecipientEmail { get; }
}
