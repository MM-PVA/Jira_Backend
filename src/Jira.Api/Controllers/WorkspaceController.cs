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
    public async Task<IActionResult> CreateAsync(CreateWorkspaceRequest request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!Guid.TryParse(userIdClaim, out var ownerId))
        {
            return Unauthorized();
        }

        var response = await _workspaceService.CreateAsync(ownerId, request, cancellationToken).ConfigureAwait(false);

        return Created(new Uri($"/api/v1/workspace/{response.Id}", UriKind.Relative), response);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllAsync(CancellationToken cancellationToken)
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!Guid.TryParse(userIdClaim, out var ownerId))
        {
            return Unauthorized();
        }

        var response = await _workspaceService.GetAllAsync(ownerId, cancellationToken).ConfigureAwait(false);

        return Ok(response);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!Guid.TryParse(userIdClaim, out var ownerId))
        {
            return Unauthorized();
        }

        var response = await _workspaceService.GetByIdAsync(id, ownerId, cancellationToken).ConfigureAwait(false);

        return Ok(response);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateAsync(Guid id, UpdateWorkspaceRequest request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!Guid.TryParse(userIdClaim, out var ownerId))
        {
            return Unauthorized();
        }

        var response = await _workspaceService.UpdateAsync(id, ownerId, request, cancellationToken).ConfigureAwait(false);

        return Ok(response);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!Guid.TryParse(userIdClaim, out var ownerId))
        {
            return Unauthorized();
        }

        await _workspaceService.DeleteAsync(id, ownerId, cancellationToken).ConfigureAwait(false);

        return NoContent();
    }
}
