namespace Jira.Application.Logging.DTOs;

public class LogGroupByIpResponse
{
    public string? IpAddress { get; set; }

    public int TotalRequests { get; set; }
}
