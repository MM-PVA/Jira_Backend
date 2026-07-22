using System.Security.Claims;

using Asp.Versioning;

using Jira.Application.Workspaces.DTOs;
using Jira.Application.Workspaces.Interfaces;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Jira.Api.Controllers;

[ApiController]
[ApiVersion(1)]
[Route("api/v{version:apiVersion}/[controller]")]
[Authorize]
public class WorkspaceController(IWorkspaceService workspaceService) : ControllerBase
{
    private readonly IWorkspaceService _workspaceService = workspaceService;

    [HttpPost]
    public async Task<IActionResult> CreateAsync(CreateWorkspaceRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!Guid.TryParse(userIdClaim, out var ownerId))
        {
            return Unauthorized();
        }

        var response = await _workspaceService.CreateAsync(ownerId, request).ConfigureAwait(false);

        return Created(new Uri($"/api/v1/workspace/{response.Id}", UriKind.Relative), response);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllAsync()
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!Guid.TryParse(userIdClaim, out var ownerId))
        {
            return Unauthorized();
        }

        var response = await _workspaceService.GetAllAsync(ownerId).ConfigureAwait(false);

        return Ok(response);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetByIdAsync(Guid id)
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!Guid.TryParse(userIdClaim, out var ownerId))
        {
            return Unauthorized();
        }

        var response = await _workspaceService.GetByIdAsync(id, ownerId).ConfigureAwait(false);

        return Ok(response);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateAsync(Guid id, UpdateWorkspaceRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!Guid.TryParse(userIdClaim, out var ownerId))
        {
            return Unauthorized();
        }

        var response = await _workspaceService.UpdateAsync(id, ownerId, request).ConfigureAwait(false);

        return Ok(response);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteAsync(Guid id)
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!Guid.TryParse(userIdClaim, out var ownerId))
        {
            return Unauthorized();
        }

        await _workspaceService.DeleteAsync(id, ownerId).ConfigureAwait(false);

        return NoContent();
    }
}
