namespace Jira.Application.Workspaces.DTOs;

public class UpdateWorkspaceRequest
{
    public required string Name { get; set; }

    public required string Description { get; set; }
}