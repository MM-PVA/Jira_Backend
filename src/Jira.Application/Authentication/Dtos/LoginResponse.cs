namespace Jira.Application.Authentication.DTOs;

public class LoginResponse
{
    public required string AccessToken { get; set; }

    public DateTime ExpiresAtUtc { get; set; }
}
