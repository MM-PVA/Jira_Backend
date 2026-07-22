using System.Security.Claims;

using Asp.Versioning;

using Jira.Application.Projects.DTOs;
using Jira.Application.Projects.Interfaces;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Jira.Api.Controllers;

[ApiController]
[ApiVersion(1)]
[Route("api/v{version:apiVersion}/workspaces/{workspaceId:guid}/projects")]
[Authorize]
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

        return Created(new Uri($"/api/v1/workspaces/{workspaceId}/projects/{response.Id}", UriKind.Relative), response);
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
