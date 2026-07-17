namespace Jira.Application.Projects.DTOs;

public class ProjectResponse
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;

    public Guid WorkspaceId { get; set; }
}
