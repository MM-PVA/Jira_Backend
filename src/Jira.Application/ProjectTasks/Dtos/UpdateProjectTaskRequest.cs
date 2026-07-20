using Jira.Domain.Enums;

namespace Jira.Application.ProjectTasks.DTOs;

public class UpdateProjectTaskRequest
{
    public required string Title { get; set; }

    public required string Description { get; set; }

    public Domain.Enums.TaskStatus Status { get; set; }

    public TaskPriority Priority { get; set; }

    public DateTime? DueDate { get; set; }
}