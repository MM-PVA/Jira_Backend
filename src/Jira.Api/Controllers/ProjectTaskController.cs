using System.Security.Claims;

using Asp.Versioning;

using Jira.Application.ProjectTasks.DTOs;
using Jira.Application.ProjectTasks.Interfaces;
using Jira.Application.ProjectTasks.Models;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Jira.Api.Controllers;

[ApiController]
[Authorize]
[ApiVersion(1)]
[ApiVersion(2)]
[Route("api/v{version:apiVersion}/workspaces/{workspaceId:guid}/projects/{projectId:guid}/tasks")]
public class ProjectTaskController(IProjectTaskService projectTaskService) : ControllerBase
{
    private readonly IProjectTaskService _projectTaskService = projectTaskService;

    [HttpPost]
    public async Task<IActionResult> CreateAsync(Guid workspaceId, Guid projectId, CreateProjectTaskRequest request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!Guid.TryParse(userIdClaim, out var ownerId))
        {
            return Unauthorized();
        }

        var model = new CreateProjectTaskModel
        {
            WorkspaceId = workspaceId,
            ProjectId = projectId,
            OwnerId = ownerId,
            Title = request.Title,
            Description = request.Description,
            Priority = request.Priority,
            AssigneeId = request.AssigneeId,
            DueDate = request.DueDate
        };

        var response = await _projectTaskService.CreateAsync(model, cancellationToken).ConfigureAwait(false);

        return Created(new Uri($"/api/v1/workspaces/{workspaceId}/projects/{projectId}/tasks/{response.Id}", UriKind.Relative), response);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllAsync(Guid workspaceId, Guid projectId, [FromQuery] GetProjectTasksRequest request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!Guid.TryParse(userIdClaim, out var ownerId))
        {
            return Unauthorized();
        }

        var model = new GetProjectTasksModel
        {
            WorkspaceId = workspaceId,
            ProjectId = projectId,
            OwnerId = ownerId,
            Search = request.Search
        };

        var response = await _projectTaskService.GetAllAsync(model, cancellationToken).ConfigureAwait(false);

        return Ok(response);
    }

    [HttpGet("{taskId:guid}")]
    public async Task<IActionResult> GetByIdAsync(Guid workspaceId, Guid projectId, Guid taskId, CancellationToken cancellationToken)
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!Guid.TryParse(userIdClaim, out var ownerId))
        {
            return Unauthorized();
        }

        var model = new GetProjectTaskByIdModel
        {
            WorkspaceId = workspaceId,
            ProjectId = projectId,
            ProjectTaskId = taskId,
            OwnerId = ownerId
        };

        var response = await _projectTaskService.GetByIdAsync(model, cancellationToken).ConfigureAwait(false);

        return Ok(response);
    }

    [HttpPut("{taskId:guid}")]
    public async Task<IActionResult> UpdateAsync(Guid workspaceId, Guid projectId, Guid taskId, UpdateProjectTaskRequest request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!Guid.TryParse(userIdClaim, out var ownerId))
        {
            return Unauthorized();
        }

        var model = new UpdateProjectTaskModel
        {
            WorkspaceId = workspaceId,
            ProjectId = projectId,
            ProjectTaskId = taskId,
            OwnerId = ownerId,
            Title = request.Title,
            Description = request.Description,
            Status = request.Status,
            Priority = request.Priority,
            DueDate = request.DueDate
        };

        var response = await _projectTaskService.UpdateAsync(model, cancellationToken).ConfigureAwait(false);

        return Ok(response);
    }

    [HttpDelete("{taskId:guid}")]
    [MapToApiVersion(1)]
    public async Task<IActionResult> DeleteV1Async(Guid workspaceId, Guid projectId, Guid taskId, CancellationToken cancellationToken)
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!Guid.TryParse(userIdClaim, out var ownerId))
        {
            return Unauthorized();
        }

        var model = new DeleteProjectTaskModel
        {
            WorkspaceId = workspaceId,
            ProjectId = projectId,
            ProjectTaskId = taskId,
            OwnerId = ownerId
        };

        await _projectTaskService.DeleteAsync(model, cancellationToken).ConfigureAwait(false);

        return NoContent();
    }

    [HttpDelete("{taskId:guid}")]
    [MapToApiVersion(2)]
    public async Task<IActionResult> DeleteV2Async(Guid workspaceId, Guid projectId, Guid taskId, [FromQuery] bool confirm, CancellationToken cancellationToken)
    {
        if (confirm != true)
        {
            throw new ArgumentException("Confirmation is required to delete the project task.");
        }

        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!Guid.TryParse(userIdClaim, out var ownerId))
        {
            return Unauthorized();
        }

        var model = new DeleteProjectTaskModel
        {
            WorkspaceId = workspaceId,
            ProjectId = projectId,
            ProjectTaskId = taskId,
            OwnerId = ownerId
        };

        await _projectTaskService.DeleteAsync(model, cancellationToken).ConfigureAwait(false);

        return NoContent();
    }
}
