namespace Jira.Application.ProjectTasks.DTOs;

public class GetProjectTasksResponse
{
    public IEnumerable<ProjectTaskResponse> Items { get; set; } = [];

    public int PageNumber { get; set; }

    public int PageSize { get; set; }

    public int TotalCount { get; set; }

    public int TotalPages { get; set; }
}
