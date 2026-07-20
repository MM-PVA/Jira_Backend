namespace Jira.Application.Projects.DTOs;

public class ProjectResponse
{
    public Guid Id { get; set; }

    public required string Name { get; set; }

    public required string Description { get; set; }

    public required string Status { get; set; }

    public Guid WorkspaceId { get; set; }
}
