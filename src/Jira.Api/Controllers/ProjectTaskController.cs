using System.Security.Claims;

using Jira.Application.ProjectTasks.DTOs;
using Jira.Application.ProjectTasks.Interfaces;
using Jira.Application.ProjectTasks.Models;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Jira.Api.Controllers;

[ApiController]
[Route("api/workspaces/{workspaceId:guid}/projects/{projectId:guid}/tasks")]
[Authorize]
public class ProjectTaskController(IProjectTaskService projectTaskService) : ControllerBase
{
    private readonly IProjectTaskService _projectTaskService = projectTaskService;

    [HttpPost]
    public async Task<IActionResult> CreateAsync(Guid workspaceId, Guid projectId, CreateProjectTaskRequest request)
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

        var response = await _projectTaskService.CreateAsync(model).ConfigureAwait(false);

        return CreatedAtAction(nameof(GetByIdAsync), new { workspaceId, projectId, taskId = response.Id }, response);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllAsync(Guid workspaceId, Guid projectId, [FromQuery] GetProjectTasksRequest request)
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

        var response = await _projectTaskService.GetAllAsync(model).ConfigureAwait(false);

        return Ok(response);
    }

    [HttpGet("{taskId:guid}")]
    public async Task<IActionResult> GetByIdAsync(Guid workspaceId, Guid projectId, Guid taskId)
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

        ArgumentNullException.ThrowIfNull(model);

        var response = await _projectTaskService.GetByIdAsync(model).ConfigureAwait(false);

        return Ok(response);
    }

    [HttpPut("{taskId:guid}")]
    public async Task<IActionResult> UpdateAsync(Guid workspaceId, Guid projectId, Guid taskId, UpdateProjectTaskRequest request)
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

        var response = await _projectTaskService.UpdateAsync(model).ConfigureAwait(false);

        return Ok(response);
    }

    [HttpDelete("{taskId:guid}")]
    public async Task<IActionResult> DeleteAsync(Guid workspaceId, Guid projectId, Guid taskId)
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

        await _projectTaskService.DeleteAsync(model).ConfigureAwait(false);

        return NoContent();
    }
}
