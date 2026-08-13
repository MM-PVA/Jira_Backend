using Jira.Domain.Entities;

namespace Jira.Application.Authentication.Interfaces;

public interface ITokenService
{
    (string Token, DateTime ExpiresAtUtc) GenerateAccessToken(User user);

    (string Token, DateTime ExpiresAtUtc) GenerateRefreshToken();

    string HashRefreshToken(string refreshToken);
}
