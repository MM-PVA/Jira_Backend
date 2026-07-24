using System.ComponentModel.DataAnnotations;

namespace Jira.Application.Projects.DTOs;

public class CreateProjectRequest
{
    [MaxLength(100)]
    public required string Name { get; set; }

    [MaxLength(250)]
    public required string Description { get; set; }

    public required string Status { get; set; }
}
