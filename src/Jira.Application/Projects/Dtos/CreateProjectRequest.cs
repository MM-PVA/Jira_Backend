namespace Jira.Application.Projects.DTOs;

public class CreateProjectRequest
{
    public required string Name { get; set; }

    public required string Description { get; set; }

    public required string Status { get; set; }
}