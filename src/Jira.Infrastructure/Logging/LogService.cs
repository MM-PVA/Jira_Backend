using System.Text.Json;

using Jira.Application.Logging.Interfaces;
using Jira.Application.Logging.Models;
using Jira.Application.Logging.DTOs;

namespace Jira.Infrastructure.Logging;

public class LogService : ILogService
{
    private readonly string _logFilePath = "C:/Users/PValiya/Jira/Logs";

    public async Task<PagedLogResponse> GetLogsAsync(LogQueryRequest request)
    {
        // Validate the request object is not null
        ArgumentNullException.ThrowIfNull(request);

        var filePath = Path.Combine(_logFilePath, $"{DateTime.UtcNow:yyyy-MM-dd}.jsonl");

        var lines = await File.ReadAllLinesAsync(filePath).ConfigureAwait(false);

        var logs = lines
            .Where(line => !string.IsNullOrWhiteSpace(line))
            .Select(line => JsonSerializer.Deserialize<RequestLog>(line)!)
            .ToList();

        if (!string.IsNullOrWhiteSpace(request.Level))
        {
            logs = logs
                .Where(log => log.Level.Equals(request.Level, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        if (!string.IsNullOrWhiteSpace(request.Method))
        {
            logs = logs
                .Where(log => log.Method.Equals(request.Method, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        if (request.StatusCode.HasValue)
        {
            logs = logs
                .Where(log => log.StatusCode == request.StatusCode.Value)
                .ToList();
        }

        if (!string.IsNullOrWhiteSpace(request.Ip))
        {
            logs = logs
                .Where(log => log.IpAddress.Equals(request.Ip, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        if (!string.IsNullOrWhiteSpace(request.Path))
        {
            logs = logs
                .Where(log => log.Path.Equals(request.Path, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        var totalRecords = logs.Count;
        var totalPages = (int)Math.Ceiling(totalRecords / (double)request.PageSize);

        var data = logs
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToList();

        return new PagedLogResponse
        {
            Page = request.Page,
            PageSize = request.PageSize,
            TotalRecords = totalRecords,
            TotalPages = totalPages,
            Data = data
        };
    }

    public async Task<List<LogGroupByIpResponse>> GroupByIpAsync(int? threshold)
    {
        var filePath = Path.Combine(_logFilePath, $"{DateTime.UtcNow:yyyy-MM-dd}.jsonl");

        var lines = await File.ReadAllLinesAsync(filePath).ConfigureAwait(false);

        var result = lines
            .Where(line => !string.IsNullOrWhiteSpace(line))
            .Select(line => JsonSerializer.Deserialize<RequestLog>(line)!) // ! is called null-forgiving operator, it tells the compiler that the value will not null.
            .GroupBy(log => log.IpAddress)
            .Select(group => new LogGroupByIpResponse
            {
                IpAddress = group.Key,
                TotalRequests = group.Count()
            })
            .ToList();

        if (threshold.HasValue)
        {
            result = result
                .Where(group => group.TotalRequests >= threshold.Value)
                .ToList();
        }

        return result;
    }
}
