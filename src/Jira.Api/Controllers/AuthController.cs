using System.Security.Claims;

using Jira.Application.Authentication.DTOs;
using Jira.Application.Authentication.Interfaces;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Jira.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IAuthService authService) : ControllerBase
{
    private readonly IAuthService _authService = authService;

    [HttpPost("register")]
    public async Task<IActionResult> RegisterAsync(RegisterRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        var response = await _authService.RegisterAsync(request).ConfigureAwait(false);

        return CreatedAtAction(nameof(GetCurrentUserAsync), response);
    }

    [HttpPost("login")]
    public async Task<IActionResult> LoginAsync(LoginRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        var response = await _authService.LoginAsync(request).ConfigureAwait(false);

        return Ok(response);
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> GetCurrentUserAsync()
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!Guid.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized();
        }

        var response = await _authService.GetCurrentUserAsync(userId).ConfigureAwait(false);

        return Ok(response);
    }
}
