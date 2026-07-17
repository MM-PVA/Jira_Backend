using System.ComponentModel.DataAnnotations;

using Jira.Domain.Common;

using Jira.Domain.Enums;

namespace Jira.Domain.Entities;

public class ProjectTask : BaseEntity
{
    [Required]
    [MaxLength(150)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string? Description { get; set; }

    public Enums.TaskStatus Status { get; set; } = Enums.TaskStatus.Todo;

    public TaskPriority Priority { get; set; } = TaskPriority.Medium;

    public DateTime? DueDate { get; set; }

    public Guid ProjectId { get; set; }

    public Project Project { get; set; } = null!;

    public Guid AssigneeId { get; set; }
}
