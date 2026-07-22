namespace Jira.Application.Workspaces.DTOs;

public class CreateWorkspaceRequest
{
    public required string Name { get; set; }

    public required string Description { get; set; }
}