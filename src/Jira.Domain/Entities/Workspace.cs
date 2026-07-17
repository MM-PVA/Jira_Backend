using Jira.Domain.Common;

using System.ComponentModel.DataAnnotations;

namespace Jira.Domain.Entities;

public class Workspace : BaseEntity
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(500)]
    public string Description { get; set; } = string.Empty;

    public Guid OwnerId { get; set; }

    // navigation property
    public User Owner { get; set; } = null!;

    // public ICollection<Project> Projects { get; set; } = new List<Project>();
    public ICollection<Project> Projects { get; } = [];
}
