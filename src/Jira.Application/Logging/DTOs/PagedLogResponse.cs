using System.Collections.ObjectModel;

using Jira.Application.Logging.Models;

namespace Jira.Application.Logging.DTOs;

public class PagedLogResponse
{
    public int Page { get; set; }

    public int PageSize { get; set; }

    public int TotalRecords { get; set; }

    public int TotalPages { get; set; }

    public IReadOnlyCollection<RequestLog> Data { get; init; } = [];
}
