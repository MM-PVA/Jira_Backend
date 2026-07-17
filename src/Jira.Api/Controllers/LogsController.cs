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

    [HttpGet("ip-group")]
    public async Task<IActionResult> GroupByIpAsync(int? threshold)
    {
        var response = await _logService.GroupByIpAsync(threshold).ConfigureAwait(false);

        return Ok(response);
    }
}
