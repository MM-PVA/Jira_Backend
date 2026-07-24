using Jira.Application.ProjectTasks.DTOs;
using Jira.Application.ProjectTasks.Interfaces;
using Jira.Application.ProjectTasks.Models;
using Jira.Domain.Entities;
using Jira.Domain.Exceptions;
using Jira.Infrastructure.Persistence;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Jira.Infrastructure.ProjectTasks;

public class ProjectTaskService(AppDbContext context, ILogger<ProjectTaskService> logger) : IProjectTaskService
{
    private readonly AppDbContext _context = context;
    private readonly ILogger<ProjectTaskService> _logger = logger;

    public async Task<ProjectTaskResponse> CreateAsync(CreateProjectTaskModel model, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(model);

        try
        {
            var project = await _context.Projects.Include(project => project.Workspace).FirstOrDefaultAsync(project => project.Id == model.ProjectId && project.WorkspaceId == model.WorkspaceId && project.Workspace.OwnerId == model.OwnerId, cancellationToken).ConfigureAwait(false);

            if (project is null)
            {
                _logger.LogWarning("Attempt to create task for non-existent project: {ProjectId}", model.ProjectId);
                throw new NotFoundException("Project not found.");
            }

            _logger.LogDebug("Creating project task with title: {Title} for project ID: {ProjectId}", model.Title, model.ProjectId);

            var projectTask = new ProjectTask
            {
                Title = model.Title,
                Description = model.Description,
                Priority = model.Priority,
                DueDate = model.DueDate,
                Status = Domain.Enums.TaskStatus.Todo,
                ProjectId = model.ProjectId,
                AssigneeId = model.AssigneeId,
                UpdatedAt = DateTime.UtcNow
            };

            _context.ProjectTasks.Add(projectTask);

            _logger.LogInformation("Project task created successfully with ID: {ProjectTaskId}", projectTask.Id);

            await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            return new ProjectTaskResponse
            {
                Id = projectTask.Id,
                Title = projectTask.Title,
                Description = projectTask.Description,
                Status = projectTask.Status,
                Priority = projectTask.Priority,
                DueDate = projectTask.DueDate,
                ProjectId = projectTask.ProjectId
            };
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("Create project task request cancelled. ProjectId: {ProjectId}", model.ProjectId);
            throw;
        }
    }

    public async Task<IEnumerable<ProjectTaskResponse>> GetAllAsync(GetProjectTasksModel model, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(model);

        try
        {
            var query = _context.ProjectTasks.Where(task => task.ProjectId == model.ProjectId && task.Project.WorkspaceId == model.WorkspaceId && task.Project.Workspace.OwnerId == model.OwnerId);

            if (!string.IsNullOrWhiteSpace(model.Search))
            {
                query = _context.ProjectTasks.Where(task => EF.Functions.Like(task.Title, $"%{model.Search}%"));
            }

            return await query
                .Select(task => new ProjectTaskResponse
                {
                    Id = task.Id,
                    Title = task.Title,
                    Description = task.Description,
                    Status = task.Status,
                    Priority = task.Priority,
                    DueDate = task.DueDate,
                    ProjectId = task.ProjectId
                })
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("Get project tasks request cancelled. ProjectId: {ProjectId}", model.ProjectId);
            throw;
        }
    }

    public async Task<ProjectTaskResponse> GetByIdAsync(GetProjectTaskByIdModel model, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(model);

        try
        {
            var task = await _context.ProjectTasks.Where(task => task.Id == model.ProjectTaskId && task.ProjectId == model.ProjectId && task.Project.WorkspaceId == model.WorkspaceId && task.Project.Workspace.OwnerId == model.OwnerId)
                .Select(task => new ProjectTaskResponse
                {
                    Id = task.Id,
                    Title = task.Title,
                    Description = task.Description,
                    Status = task.Status,
                    Priority = task.Priority,
                    DueDate = task.DueDate,
                    ProjectId = task.ProjectId
                })
                .FirstOrDefaultAsync(cancellationToken)
                .ConfigureAwait(false);

            if (task is null)
            {
                _logger.LogWarning("Attempt to get non-existent project task: {ProjectTaskId}", model.ProjectTaskId);
                throw new NotFoundException("Task not found.");
            }

            return task;
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("Get project task request cancelled. ProjectTaskId: {ProjectTaskId}", model.ProjectTaskId);
            throw;
        }
    }

    public async Task<ProjectTaskResponse> UpdateAsync(UpdateProjectTaskModel model, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(model);

        try
        {
            var task = await _context.ProjectTasks.FirstOrDefaultAsync(task => task.Id == model.ProjectTaskId && task.ProjectId == model.ProjectId && task.Project.WorkspaceId == model.WorkspaceId && task.Project.Workspace.OwnerId == model.OwnerId, cancellationToken).ConfigureAwait(false);

            if (task is null)
            {
                _logger.LogWarning("Attempt to update non-existent project task: {ProjectTaskId}", model.ProjectTaskId);
                throw new NotFoundException("Task not found.");
            }

            _logger.LogDebug("Updating project task with ID: {ProjectTaskId}", task.Id);

            task.Title = model.Title;
            task.Description = model.Description;
            task.Status = model.Status;
            task.Priority = model.Priority;
            task.DueDate = model.DueDate;
            task.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            _logger.LogInformation("Project task updated successfully with ID: {ProjectTaskId}", task.Id);

            return new ProjectTaskResponse
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                Status = task.Status,
                Priority = task.Priority,
                DueDate = task.DueDate,
                ProjectId = task.ProjectId
            };
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("Update project task request cancelled. ProjectTaskId: {ProjectTaskId}", model.ProjectTaskId);
            throw;
        }
    }

    public async Task DeleteAsync(DeleteProjectTaskModel model, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(model);

        try
        {
            var task = await _context.ProjectTasks.FirstOrDefaultAsync(task => task.Id == model.ProjectTaskId && task.ProjectId == model.ProjectId && task.Project.WorkspaceId == model.WorkspaceId && task.Project.Workspace.OwnerId == model.OwnerId, cancellationToken).ConfigureAwait(false);

            if (task is null)
            {
                _logger.LogWarning("Attempt to delete non-existent project task: {ProjectTaskId}", model.ProjectTaskId);
                throw new NotFoundException("Task not found.");
            }

            _context.ProjectTasks.Remove(task);

            _logger.LogInformation("Project task deleted successfully with ID: {ProjectTaskId}", task.Id);

            await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("Delete project task request cancelled. ProjectTaskId: {ProjectTaskId}", model.ProjectTaskId);
            throw;
        }
    }
}

