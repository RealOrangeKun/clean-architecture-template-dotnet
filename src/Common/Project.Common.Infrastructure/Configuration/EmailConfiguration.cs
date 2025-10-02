namespace Project.Common.Infrastructure.Configuration;

public sealed class EmailConfiguration
{
    public const string SectionName = "FluentEmail";

    public string From { get; set; } = string.Empty;
    public string Host { get; set; } = string.Empty;
    public int Port { get; set; }
}
