using Jira.Application.Authentication.DTOs;
using Jira.Application.Authentication.Interfaces;
using Jira.Domain.Entities;
using Jira.Domain.Exceptions;
using Jira.Infrastructure.Persistence;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Jira.Infrastructure.Authentication;

public class AuthService(AppDbContext context, ITokenService tokenService, ILogger<AuthService> logger) : IAuthService
{
    private readonly AppDbContext _context = context;
    private readonly PasswordHasher<User> _passwordHasher = new();
    private readonly ITokenService _tokenService = tokenService;
    private readonly ILogger<AuthService> _logger = logger;

    public async Task<RegisterResponse> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        try
        {
            var existingUser = await _context.Users.FirstOrDefaultAsync(x => x.Email == request.Email, cancellationToken).ConfigureAwait(false);

            if (existingUser != null)
            {
                _logger.LogWarning("Attempt to register with an existing email: {Email}", request.Email);
                throw new ConflictException("Email already exists.");
            }

            var user = new User
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email
            };

            user.PasswordHash = _passwordHasher.HashPassword(user, request.Password);

            _context.Users.Add(user);

            await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            return new RegisterResponse
            {
                UserId = user.Id,
                Message = "User registered successfully."
            };
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("Registration request cancelled by {Email}", request.Email);
            throw;
        }
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        _logger.LogInformation("Login request received from {Email}", request.Email);

        try
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == request.Email, cancellationToken).ConfigureAwait(false);

            if (user == null)
            {
                throw new UnauthorizedException("Invalid email or password.");
            }

            var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);

            if (result == PasswordVerificationResult.Failed)
            {
                throw new UnauthorizedException("Invalid email or password.");
            }

            var (accessToken, expiresAtUtc) = _tokenService.GenerateToken(user);

            return new LoginResponse
            {
                AccessToken = accessToken,
                ExpiresAtUtc = expiresAtUtc
            };
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("Login request cancelled by {Email}", request.Email);
            throw;
        }
    }

    public async Task<CurrentUserResponse> GetCurrentUserAsync(Guid userId, CancellationToken cancellationToken)
    {
        try
        {
            var user = await _context.Users.FindAsync([userId], cancellationToken).ConfigureAwait(false);

            if (user == null)
            {
                _logger.LogWarning("Attempt to get non-existent user with ID: {UserId}", userId);
                throw new NotFoundException("User not found.");
            }

            _logger.LogInformation("Current user retrieved successfully with ID: {UserId}", userId);

            return new CurrentUserResponse
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email
            };
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("GetCurrentUser request cancelled by user ID: {UserId}", userId);
            throw;
        }
    }
}
