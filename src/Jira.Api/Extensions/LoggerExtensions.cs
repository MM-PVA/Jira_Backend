namespace Jira.Api.Extensions;

internal static class LoggerExtensions
{
    internal static void LogHttpRequest(this ILogger logger, HttpContext context)
    {
        ArgumentNullException.ThrowIfNull(logger);
        ArgumentNullException.ThrowIfNull(context);

        logger.LogInformation("[Request LOGS] : [{RemoteIpAddress}]/[{TraceId}]\n\tMethod: {Method}\n\tPath: {Path}",
            context.Connection.RemoteIpAddress,
            context.TraceIdentifier,
            context.Request.Method,
            context.Request.Path);
    }

    internal static void LogHttpResponse(this ILogger logger, HttpContext context, long elapsedMilliseconds)
    {
        ArgumentNullException.ThrowIfNull(logger);
        ArgumentNullException.ThrowIfNull(context);

        var statusCode = context.Response.StatusCode;

        var logLevel = context.Response.StatusCode.GetLogLevel();

        logger.Log(logLevel,
            "[Response LOGS] : [{TraceId}]\n\tStatusCode: {StatusCode}\n\tCompleted in {ElapsedMilliseconds} ms",
            context.TraceIdentifier,
            statusCode,
            elapsedMilliseconds);
    }
}
