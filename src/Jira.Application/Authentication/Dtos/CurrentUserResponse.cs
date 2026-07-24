using System.ComponentModel.DataAnnotations;

namespace Jira.Application.Authentication.DTOs;

public class CurrentUserResponse
{
    public Guid Id { get; set; }

    [MaxLength(50)]
    public required string FirstName { get; set; }

    [MaxLength(100)]
    public required string LastName { get; set; }

    [EmailAddress]
    public required string Email { get; set; }
}
