using System.ComponentModel.DataAnnotations;

using Jira.Domain.Enums;

namespace Jira.Application.ProjectTasks.DTOs;

public class CreateProjectTaskRequest
{
    [MaxLength(100)]
    public required string Title { get; set; }

    [MaxLength(250)]
    public required string Description { get; set; }

    public TaskPriority Priority { get; set; }

    public Guid AssigneeId { get; set; }

    public DateTime? DueDate { get; set; }
}
