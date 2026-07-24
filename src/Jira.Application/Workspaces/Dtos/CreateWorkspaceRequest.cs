using System.ComponentModel.DataAnnotations;

namespace Jira.Application.Workspaces.DTOs;

public class CreateWorkspaceRequest
{
    [MaxLength(100)]
    public required string Name { get; set; }

    [MaxLength(250)]
    public required string Description { get; set; }
}
