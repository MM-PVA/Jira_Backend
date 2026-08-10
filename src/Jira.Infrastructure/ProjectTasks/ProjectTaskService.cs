using Jira.Application.ProjectTasks.DTOs;
using Jira.Application.ProjectTasks.Interfaces;
using Jira.Application.ProjectTasks.Interfaces.Repositories;
using Jira.Application.ProjectTasks.Models;
using Jira.Domain.Entities;
using Jira.Domain.Exceptions;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Jira.Infrastructure.ProjectTasks;

public class ProjectTaskService(IProjectTaskRepository projectTaskRepository, ILogger<ProjectTaskService> logger) : IProjectTaskService
{
    private readonly IProjectTaskRepository _projectTaskRepository = projectTaskRepository;
    private readonly ILogger<ProjectTaskService> _logger = logger;

    public async Task<ProjectTaskResponse> CreateAsync(CreateProjectTaskModel model, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(model);

        try
        {
            var projectExists = await _projectTaskRepository.ProjectExistsAsync(model.ProjectId, model.WorkspaceId, model.OwnerId, cancellationToken).ConfigureAwait(false);

            if (!projectExists)
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
                DueDate = model.DueDate.HasValue ? DateTime.SpecifyKind(model.DueDate.Value, DateTimeKind.Utc) : null,
                Status = Domain.Enums.TaskStatus.Todo,
                ProjectId = model.ProjectId,
                AssigneeId = model.AssigneeId,
                UpdatedAt = DateTime.UtcNow
            };

            await _projectTaskRepository.AddAsync(projectTask, cancellationToken).ConfigureAwait(false);

            await _projectTaskRepository.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            _logger.LogInformation("Project task created successfully with ID: {ProjectTaskId}", projectTask.Id);

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
        catch (DbUpdateException exception)
        {
            _logger.LogError(
                exception,
                "Database error while creating project task. Inner exception: {InnerException}",
                exception.InnerException?.Message);

            throw;
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("Create project task request cancelled. ProjectId: {ProjectId}", model.ProjectId);
            throw;
        }
    }

    public async Task<GetProjectTasksResponse> GetAllAsync(GetProjectTasksModel model, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(model);

        try
        {
            var projectExists = await _projectTaskRepository.ProjectExistsAsync(model.ProjectId, model.WorkspaceId, model.OwnerId, cancellationToken).ConfigureAwait(false);

            if (!projectExists)
            {
                _logger.LogWarning("Attempt to get tasks for non-existent project: {ProjectId}", model.ProjectId);

                throw new NotFoundException("Project not found.");
            }

            var (tasks, totalCount) = await _projectTaskRepository.GetAllAsync(model.ProjectId, model.Search, model.PageNumber, model.PageSize, cancellationToken).ConfigureAwait(false);

            var items = tasks.Select(task => new ProjectTaskResponse
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                Status = task.Status,
                Priority = task.Priority,
                DueDate = task.DueDate,
                ProjectId = task.ProjectId
            });

            var totalPages = (int)Math.Ceiling(totalCount / (double)model.PageSize);

            return new GetProjectTasksResponse
            {
                Items = items,
                PageNumber = model.PageNumber,
                PageSize = model.PageSize,
                TotalCount = totalCount,
                TotalPages = totalPages
            };
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
            var projectExists = await _projectTaskRepository.ProjectExistsAsync(model.ProjectId, model.WorkspaceId, model.OwnerId, cancellationToken).ConfigureAwait(false);

            if (!projectExists)
            {
                _logger.LogWarning("Attempt to get task for non-existent project: {ProjectId}", model.ProjectId);
                throw new NotFoundException("Project not found.");
            }

            var task = await _projectTaskRepository.GetByIdAsync(model.ProjectTaskId, model.ProjectId, cancellationToken).ConfigureAwait(false);

            if (task is null)
            {
                _logger.LogWarning("Attempt to get non-existent project task: {ProjectTaskId}", model.ProjectTaskId);
                throw new NotFoundException("Task not found.");
            }

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
            _logger.LogWarning("Get project task request cancelled. ProjectTaskId: {ProjectTaskId}", model.ProjectTaskId);
            throw;
        }
    }

    public async Task<ProjectTaskResponse> UpdateAsync(UpdateProjectTaskModel model, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(model);

        try
        {
            var projectExists = await _projectTaskRepository.ProjectExistsAsync(model.ProjectId, model.WorkspaceId, model.OwnerId, cancellationToken).ConfigureAwait(false);

            if (!projectExists)
            {
                _logger.LogWarning("Attempt to update task for non-existent project: {ProjectId}", model.ProjectId);
                throw new NotFoundException("Project not found.");
            }

            var task = await _projectTaskRepository.GetByIdAsync(model.ProjectTaskId, model.ProjectId, cancellationToken).ConfigureAwait(false);

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
            task.DueDate = model.DueDate.HasValue ? DateTime.SpecifyKind(model.DueDate.Value, DateTimeKind.Utc) : null;
            task.UpdatedAt = DateTime.UtcNow;

            await _projectTaskRepository.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

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
            var projectExists = await _projectTaskRepository.ProjectExistsAsync(model.ProjectId, model.WorkspaceId, model.OwnerId, cancellationToken).ConfigureAwait(false);

            if (!projectExists)
            {
                _logger.LogWarning("Attempt to delete task for non-existent project: {ProjectId}", model.ProjectId);
                throw new NotFoundException("Project not found.");
            }

            var task = await _projectTaskRepository.GetByIdAsync(model.ProjectTaskId, model.ProjectId, cancellationToken).ConfigureAwait(false);

            if (task is null)
            {
                _logger.LogWarning("Attempt to delete non-existent project task: {ProjectTaskId}", model.ProjectTaskId);
                throw new NotFoundException("Task not found.");
            }

            _projectTaskRepository.Remove(task);

            await _projectTaskRepository.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            _logger.LogInformation("Project task deleted successfully with ID: {ProjectTaskId}", task.Id);
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("Delete project task request cancelled. ProjectTaskId: {ProjectTaskId}", model.ProjectTaskId);
            throw;
        }
    }
}
