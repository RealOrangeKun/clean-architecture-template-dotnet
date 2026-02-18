namespace Project.Modules.Notifications.Application.Emails.Templates;

public static class EmailTemplatePaths
{
    private static readonly string TemplateBasePath = Path.Combine(AppContext.BaseDirectory, "Emails", "Templates");

    public static class Welcome
    {
        public static readonly string TemplatePath = Path.Combine(TemplateBasePath, "Welcome", "WelcomeEmail.cshtml");
    }

    public static class PasswordReset
    {
        public static readonly string TemplatePath = Path.Combine(TemplateBasePath, "PasswordReset", "PasswordResetEmail.cshtml");
    }
}
