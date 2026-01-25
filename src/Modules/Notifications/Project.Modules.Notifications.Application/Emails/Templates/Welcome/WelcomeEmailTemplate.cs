using Project.Modules.Notifications.Application.Abstractions.Emails;

namespace Project.Modules.Notifications.Application.Emails.Templates.Welcome;

public sealed class WelcomeEmailTemplate(
    string recipientEmail,
    string firstName,
    string lastName,
    string? loginUrl = null) : EmailTemplate(recipientEmail)
{
    public override string TemplatePath => EmailTemplatePaths.Welcome.TemplatePath;

    public override string Subject => $"Welcome to Our Platform, {FirstName}!";

    public string FirstName { get; } = firstName ?? throw new ArgumentNullException(nameof(firstName));
    public string LastName { get; } = lastName ?? throw new ArgumentNullException(nameof(lastName));
    public string FullName { get; } = $"{firstName} {lastName}";
    public string LoginUrl { get; } = loginUrl ?? "https://yourapp.com/login";
    public int CurrentYear { get; } = DateTime.UtcNow.Year;

}
