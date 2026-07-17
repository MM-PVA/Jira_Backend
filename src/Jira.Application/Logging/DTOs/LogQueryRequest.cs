using System.ComponentModel.DataAnnotations;

namespace Jira.Application.Logging.DTOs;

public class LogQueryRequest
{
    public string? Level { get; set; }

    public string? Method { get; set; }

    public int? StatusCode { get; set; }

    public string? Ip { get; set; }

    public string? Path { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Page must be greater than 0.")]
    public int Page { get; set; } = 1;

    [Range(1, 100, ErrorMessage = "PageSize must be between 1 and 100.")]
    public int PageSize { get; set; } = 5;
}
