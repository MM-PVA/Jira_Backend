namespace Jira.Application.Logging.DTOs;

public class LogGroupByIpResponse
{
    public string IpAddress { get; set; } = string.Empty;

    public int TotalRequests { get; set; }
}
