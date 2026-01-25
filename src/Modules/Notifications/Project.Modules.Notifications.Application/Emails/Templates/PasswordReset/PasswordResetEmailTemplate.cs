using Project.Modules.Notifications.Application.Abstractions.Emails;

namespace Project.Modules.Notifications.Application.Emails.Templates.PasswordReset;

public sealed class PasswordResetEmailTemplate(
    string recipientEmail,
    string userName,
    string resetUrl,
    DateTime expiresAt) : EmailTemplate(recipientEmail)
{
    public override string TemplatePath => EmailTemplatePaths.PasswordReset.TemplatePath;

    public override string Subject => "Reset Your Password";

    public string UserName { get; } = userName ?? throw new ArgumentNullException(nameof(userName));
    public string ResetUrl { get; } = resetUrl ?? throw new ArgumentNullException(nameof(resetUrl));
    public DateTime ExpiresAt { get; } = expiresAt;

    public string ExpiryTimeRemaining
    {
        get
        {
            TimeSpan timeRemaining = ExpiresAt - DateTime.UtcNow;
            if (timeRemaining.TotalHours >= 1)
                return $"{timeRemaining.TotalHours:F0} hours";
            else
                return $"{timeRemaining.TotalMinutes:F0} minutes";
        }
    }

    public int CurrentYear => DateTime.UtcNow.Year;
}
