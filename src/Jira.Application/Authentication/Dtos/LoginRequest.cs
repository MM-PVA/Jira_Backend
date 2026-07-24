using System.ComponentModel.DataAnnotations;

namespace Jira.Application.Authentication.DTOs;

public class LoginRequest
{
    [EmailAddress]
    public required string Email { get; set; }

    [MinLength(6)]
    public required string Password { get; set; }
}
