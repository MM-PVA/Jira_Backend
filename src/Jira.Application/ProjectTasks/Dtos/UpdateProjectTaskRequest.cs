using System.ComponentModel.DataAnnotations;

using Jira.Domain.Enums;

namespace Jira.Application.ProjectTasks.DTOs;

public class UpdateProjectTaskRequest
{
    [MaxLength(100)]
    public required string Title { get; set; }

    [MaxLength(250)]
    public required string Description { get; set; }

    public Domain.Enums.TaskStatus Status { get; set; }

    public TaskPriority Priority { get; set; }

    public DateTime? DueDate { get; set; }
}
