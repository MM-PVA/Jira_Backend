namespace Jira.Infrastructure.Logging;

public sealed class LoggingSettings
{
    public const string SectionName = "LoggingSettings";

    public required string LogDirectory { get; init; }
}
