using System.ComponentModel.DataAnnotations;

namespace Jira.Application.Authentication.DTOs;

public class RegisterRequest
{
    [MaxLength(50)]
    public required string FirstName { get; set; }

    [MaxLength(100)]
    public required string LastName { get; set; }

    [EmailAddress]
    public required string Email { get; set; }

    [MinLength(6)]
    public required string Password { get; set; }
}
