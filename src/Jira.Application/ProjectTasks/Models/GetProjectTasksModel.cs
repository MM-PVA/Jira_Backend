namespace Jira.Application.ProjectTasks.Models;

public class GetProjectTasksModel
{
    public Guid WorkspaceId { get; set; }

    public Guid ProjectId { get; set; }

    public Guid OwnerId { get; set; }

    public string? Search { get; set; }

    public int PageNumber { get; set; }

    public int PageSize { get; set; }
}
