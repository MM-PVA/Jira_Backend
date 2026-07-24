using System.ComponentModel.DataAnnotations;

namespace Jira.Application.ProjectTasks.DTOs;

public class GetProjectTasksRequest
{
    [MaxLength(100)]
    public string? Search { get; set; }
}
