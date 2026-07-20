namespace Jira.Application.Workspaces.DTOs;

public class WorkspaceResponse
{
    public Guid Id { get; set; }

    public required string Name { get; set; }

    public required string Description { get; set; }

    public Guid OwnerId { get; set; }
}