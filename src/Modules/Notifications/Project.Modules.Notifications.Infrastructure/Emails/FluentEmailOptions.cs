namespace Project.Modules.Notifications.Infrastructure.Emails;

public class FluentEmailOptions
{
    public static string SectionName => "FluentEmail";
    public string From { get; set; } = string.Empty;
    public string Host { get; set; } = string.Empty;
    public int Port { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
