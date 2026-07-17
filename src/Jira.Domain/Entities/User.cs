using System.ComponentModel.DataAnnotations;

using Jira.Domain.Common;

namespace Jira.Domain.Entities;

public class User : BaseEntity
{
    [Required]
    [MaxLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string LastName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [MaxLength(255)]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string PasswordHash { get; set; } = string.Empty;

    public ICollection<Workspace> OwnedWorkspaces { get; } = [];

    public ICollection<ProjectTask> AssignedTasks { get; } = [];
}
