using Jira.Domain.Common;

using System.ComponentModel.DataAnnotations;

namespace Jira.Domain.Entities;

public class Project : BaseEntity
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(500)]
    public string Description { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;

    public Guid WorkspaceId { get; set; }

    public Workspace Workspace { get; set; } = null!;

    public ICollection<ProjectTask> Tasks { get; } = [];
}
