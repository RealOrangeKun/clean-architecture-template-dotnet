namespace Project.Common.Infrastructure.Configuration;

public sealed class ConnectionStrings
{
    public const string SectionName = "ConnectionStrings";

    public string Database { get; set; } = string.Empty;
    public string Redis { get; set; } = string.Empty;
}
