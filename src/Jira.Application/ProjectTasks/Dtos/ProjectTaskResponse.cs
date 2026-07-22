using Jira.Domain.Enums;

using System.Text.Json.Serialization;

namespace Jira.Application.ProjectTasks.DTOs;

public class ProjectTaskResponse
{
    public Guid Id { get; set; }

    public required string Title { get; set; }

    public required string? Description { get; set; }

    public Domain.Enums.TaskStatus Status { get; set; }

    public TaskPriority Priority { get; set; }

    // Custom Property Name Serialization
    [JsonPropertyName("due_date")]
    public DateTime? DueDate { get; set; }

    [JsonIgnore]
    public Guid ProjectId { get; set; }

    // Ignore Property During Serialization
    // [JsonIgnore]
    // public bool IsOverdue => DueDate.HasValue && DueDate.Value < DateTime.UtcNow;
}
