using System.Diagnostics;
using Jira.Api.Extensions;
using Jira.Application.Logging.Models;
using System.Text.Json;

namespace Jira.Api.Middleware;

public class RequestResponseLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestResponseLoggingMiddleware> _logger;

    private readonly string _logFilePath = "C:/Users/PValiya/Jira/Logs";

    public RequestResponseLoggingMiddleware(RequestDelegate next, ILogger<RequestResponseLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();

        _logger.LogHttpRequest(context);

        try
        {
            await _next(context);
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

            await File.AppendAllTextAsync(filePath, json + Environment.NewLine);
        }
    }
}