namespace Project.Common.Infrastructure.Configuration;

public sealed class RabbitMqConfiguration
{
    public const string SectionName = "RabbitMQ";

    public string Host { get; set; } = string.Empty;
    public string VirtualHost { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
