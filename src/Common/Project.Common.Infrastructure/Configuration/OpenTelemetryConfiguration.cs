namespace Project.Common.Infrastructure.Configuration;

public sealed class OpenTelemetryOptions
{
    public const string SectionName = "OpenTelemetry";
    public readonly Dictionary<string, object> ServiceAttributes = new()
    {
        ["host.name"] = Environment.MachineName
    };
    public string ServiceName { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
}

