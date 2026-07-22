namespace Jira.Api.Extensions;

internal static class HttpStatusCodeExtensions
{
    public static LogLevel GetLogLevel(this int statusCode)
    {
        return statusCode switch
        {
            >= 500 => LogLevel.Error,
            >= 400 => LogLevel.Warning,
            _ => LogLevel.Information
        };
    }
}
