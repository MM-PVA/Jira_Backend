using System.Diagnostics;
using Jira.Api.Extensions;
using Jira.Application.Logging.Models;
using System.Text.Json;

namespace Jira.Api.Middleware;

public sealed class RequestResponseLoggingMiddleware(RequestDelegate next, ILogger<RequestResponseLoggingMiddleware> logger)
{
    private readonly RequestDelegate _next = next;
    private readonly ILogger<RequestResponseLoggingMiddleware> _logger = logger;

    private readonly string _logFilePath = "C:/Users/PValiya/Jira/Logs";

    public async Task InvokeAsync(HttpContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        var stopwatch = Stopwatch.StartNew();

        _logger.LogHttpRequest(context);

        try
        {
            await _next(context).ConfigureAwait(false);
        }
        finally
        {
            stopwatch.Stop();

            _logger.LogHttpResponse(context, stopwatch.ElapsedMilliseconds);

            var requestLog = new RequestLog
            {
                Timestamp = DateTime.UtcNow,
                TraceId = context.TraceIdentifier,
                Method = context.Request.Method,
                Path = context.Request.Path,
                IpAddress = context.Connection.RemoteIpAddress?.ToString() ?? "Unknown",
                StatusCode = context.Response.StatusCode,
                ElapsedMilliseconds = stopwatch.ElapsedMilliseconds,
                Level = context.Response.StatusCode.GetLogLevel().ToString()
            };

            var json = JsonSerializer.Serialize(requestLog);

            var filePath = Path.Combine(_logFilePath, $"{DateTime.UtcNow:yyyy-MM-dd}.jsonl");

            await File.AppendAllTextAsync(filePath, json + Environment.NewLine).ConfigureAwait(false);
        }
    }
}
