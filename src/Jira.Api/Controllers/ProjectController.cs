using System.Security.Claims;

using Jira.Application.Projects.DTOs;
using Jira.Application.Projects.Interfaces;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Jira.Api.Controllers;

[ApiController]
[Route("api/workspaces/{workspaceId:guid}/projects")]
[Authorize]
#pragma warning disable CA1515
public class ProjectController(IProjectService projectService) : ControllerBase
{
    private readonly IProjectService _projectService = projectService;

    [HttpPost]
    public async Task<IActionResult> CreateAsync(Guid workspaceId, CreateProjectRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!Guid.TryParse(userIdClaim, out var ownerId))
        {
            return Unauthorized();
        }

        var response = await _projectService.CreateAsync(workspaceId, ownerId, request).ConfigureAwait(false);

        return CreatedAtAction(nameof(GetByIdAsync), new { workspaceId, projectId = response.Id }, response);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllAsync(Guid workspaceId)
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!Guid.TryParse(userIdClaim, out var ownerId))
        {
            return Unauthorized();
        }

        var response = await _projectService.GetAllAsync(workspaceId, ownerId).ConfigureAwait(false);

        return Ok(response);
    }

    [HttpGet("{projectId:guid}")]
    public async Task<IActionResult> GetByIdAsync(Guid workspaceId, Guid projectId)
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!Guid.TryParse(userIdClaim, out var ownerId))
        {
            return Unauthorized();
        }

        var response = await _projectService.GetByIdAsync(projectId, workspaceId, ownerId).ConfigureAwait(false);

        return Ok(response);
    }

    [HttpPut("{projectId:guid}")]
    public async Task<IActionResult> UpdateAsync(Guid workspaceId, Guid projectId, UpdateProjectRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!Guid.TryParse(userIdClaim, out var ownerId))
        {
            return Unauthorized();
        }

        var response = await _projectService.UpdateAsync(projectId, workspaceId, ownerId, request).ConfigureAwait(false);

        return Ok(response);
    }

    [HttpDelete("{projectId:guid}")]
    public async Task<IActionResult> DeleteAsync(Guid workspaceId, Guid projectId)
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!Guid.TryParse(userIdClaim, out var ownerId))
        {
            return Unauthorized();
        }

        await _projectService.DeleteAsync(projectId, workspaceId, ownerId).ConfigureAwait(false);

        return NoContent();
    }
}
#pragma warning restore CA1515
