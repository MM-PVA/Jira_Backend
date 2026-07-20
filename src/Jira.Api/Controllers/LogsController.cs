using Microsoft.AspNetCore.Mvc;

using Jira.Application.Logging.DTOs;

using Jira.Application.Logging.Interfaces;

namespace Jira.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LogsController(ILogService logService) : ControllerBase
{
    private readonly ILogService _logService = logService;

    [HttpGet]
    public async Task<IActionResult> GetLogsAsync([FromQuery] LogQueryRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        var response = await _logService.GetLogsAsync(request).ConfigureAwait(false);

        return Ok(response);
    }

    [HttpPost]
    public async Task<IActionResult> SearchLogsAsync([FromBody] LogQueryRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        var result = await _logService.GetLogsAsync(request).ConfigureAwait(false);

        return Ok(result);
    }

    [HttpGet("header-search")]
    public async Task<IActionResult> SearchLogsFromHeaderAsync(
        [FromHeader(Name = "Level")] string? level,
        [FromHeader(Name = "Method")] string? method,
        [FromHeader(Name = "StatusCode")] int? statusCode,
        [FromHeader(Name = "Ip")] string? ip,
        [FromHeader(Name = "Path")] string? path
    )
    {
        var request = new LogQueryRequest
        {
            Level = level,
            Method = method,
            StatusCode = statusCode,
            Ip = ip,
            Path = path,
        };

        var result = await _logService.GetLogsAsync(request).ConfigureAwait(false);

        return Ok(result);
    }

    [HttpGet("ip-group")]
    public async Task<IActionResult> GroupByIpAsync(int? threshold)
    {
        var response = await _logService.GroupByIpAsync(threshold).ConfigureAwait(false);

        return Ok(response);
    }
}
