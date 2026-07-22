namespace Jira.Application.Authentication.DTOs;

public class RegisterResponse
{
    public Guid UserId { get; set; }

    public required string Message { get; set; }
}
