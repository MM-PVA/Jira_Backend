using Jira.Application.Authentication.DTOs;
using Jira.Application.Authentication.Interfaces;
using Jira.Domain.Entities;
using Jira.Domain.Exceptions;
using Jira.Infrastructure.Persistence;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Jira.Infrastructure.Authentication;

public class AuthService : IAuthService
{
    private readonly AppDbContext _context;
    private readonly PasswordHasher<User> _passwordHasher;
    private readonly ITokenService _tokenService;
    private readonly ILogger<AuthService> _logger;

    public AuthService(AppDbContext context, ITokenService tokenService, ILogger<AuthService> logger)
    {
        _context = context;
        _passwordHasher = new PasswordHasher<User>();
        _tokenService = tokenService;
        _logger = logger;
    }

    public async Task<RegisterResponse> RegisterAsync(RegisterRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        var existingUser = await _context.Users
            .FirstOrDefaultAsync(x => x.Email == request.Email).ConfigureAwait(false);

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

        await _context.SaveChangesAsync().ConfigureAwait(false);

        _logger.LogInformation("User registered successfully with email: {Email}", request.Email);

        return new RegisterResponse
        {
            UserId = user.Id,
            Message = "User registered successfully."
        };
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        var user = await _context.Users
            .FirstOrDefaultAsync(x => x.Email == request.Email).ConfigureAwait(false);

        if (user == null)
        {
            _logger.LogWarning("Login attempt failed for non-existent email: {Email}", request.Email);
            throw new UnauthorizedException("Invalid email or password.");
        }

        var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);

        if (result == PasswordVerificationResult.Failed)
        {
            _logger.LogWarning("Failed login attempt for email: {Email}", request.Email);
            throw new UnauthorizedException("Invalid email or password.");
        }

        var (accessToken, expiresAtUtc) = _tokenService.GenerateToken(user);

        return new LoginResponse
        {
            AccessToken = accessToken,
            ExpiresAtUtc = expiresAtUtc
        };
    }

    public async Task<CurrentUserResponse> GetCurrentUserAsync(Guid userId)
    {
        var user = await _context.Users.FindAsync(userId).ConfigureAwait(false);

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
}
