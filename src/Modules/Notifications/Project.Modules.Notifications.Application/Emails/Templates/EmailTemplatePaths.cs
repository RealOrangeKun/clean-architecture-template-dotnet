namespace Project.Modules.Notifications.Application.Emails.Templates;

public static class EmailTemplatePaths
{
    private const string TemplateBasePath = "Emails/Templates";

    public static class Welcome
    {
        public const string TemplatePath = $"{TemplateBasePath}/Welcome/WelcomeEmail.cshtml";
    }

    public static class PasswordReset
    {
        public const string TemplatePath = $"{TemplateBasePath}/PasswordReset/PasswordResetEmail.cshtml";
    }
}
