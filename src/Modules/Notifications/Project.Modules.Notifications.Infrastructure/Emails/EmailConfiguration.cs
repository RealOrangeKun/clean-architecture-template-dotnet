namespace Project.Modules.Notifications.Infrastructure.Emails;

public class EmailConfiguration
{
    public string From { get; set; } = string.Empty;
    public string Host { get; set; } = string.Empty;
    public int Port { get; set; }
}
