using Jira.Domain.Enums;

namespace Jira.Application.ProjectTasks.DTOs;

public class CreateProjectTaskRequest
{
    public required string Title { get; set; }

    public required string Description { get; set; }

    public TaskPriority Priority { get; set; }

    public Guid AssigneeId { get; set; }

    public DateTime? DueDate { get; set; }
}