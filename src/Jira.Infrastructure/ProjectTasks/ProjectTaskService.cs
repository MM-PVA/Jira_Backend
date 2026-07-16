using Jira.Application.ProjectTasks.DTOs;
using Jira.Application.ProjectTasks.Interfaces;
using Jira.Application.ProjectTasks.Models;
using Jira.Domain.Entities;
using Jira.Domain.Exceptions;
using Jira.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Jira.Infrastructure.ProjectTasks;

public class ProjectTaskService : IProjectTaskService
{
    private readonly AppDbContext _context;
    private readonly ILogger<ProjectTaskService> _logger;

    public ProjectTaskService(AppDbContext context, ILogger<ProjectTaskService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<ProjectTaskResponse> CreateAsync(CreateProjectTaskModel model)
    {
        var project = await _context.Projects.Include(project => project.Workspace).FirstOrDefaultAsync(project =>
            project.Id == model.ProjectId && project.WorkspaceId == model.WorkspaceId && project.Workspace.OwnerId == model.OwnerId);

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

        await _context.SaveChangesAsync();

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

    public async Task<IEnumerable<ProjectTaskResponse>> GetAllAsync(GetProjectTasksModel model)
    {
        return await _context.ProjectTasks.Where(task =>
            task.ProjectId == model.ProjectId && task.Project.WorkspaceId == model.WorkspaceId && task.Project.Workspace.OwnerId == model.OwnerId)
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
            .ToListAsync();
    }

    public async Task<ProjectTaskResponse> GetByIdAsync(GetProjectTaskByIdModel model)
    {
        var task = await _context.ProjectTasks.Where(task =>
                task.Id == model.ProjectTaskId && task.ProjectId == model.ProjectId && task.Project.WorkspaceId == model.WorkspaceId && task.Project.Workspace.OwnerId == model.OwnerId)
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
            .FirstOrDefaultAsync();

        // if (task is null)
        // {
        //     _logger.LogWarning("Attempt to get non-existent project task: {ProjectTaskId}", model.ProjectTaskId);
        //     throw new NotFoundException("Task not found.");
        // }

        _logger.LogInformation("Project task retrieved successfully with ID: {ProjectTaskId}", task.Id);

        return task;
    }

    public async Task<ProjectTaskResponse> UpdateAsync(UpdateProjectTaskModel model)
    {
        var task = await _context.ProjectTasks.FirstOrDefaultAsync(task =>
            task.Id == model.ProjectTaskId && task.ProjectId == model.ProjectId && task.Project.WorkspaceId == model.WorkspaceId && task.Project.Workspace.OwnerId == model.OwnerId);

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

        await _context.SaveChangesAsync();

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

    public async Task DeleteAsync(DeleteProjectTaskModel model)
    {
        var task = await _context.ProjectTasks.FirstOrDefaultAsync(task =>
                task.Id == model.ProjectTaskId && task.ProjectId == model.ProjectId && task.Project.WorkspaceId == model.WorkspaceId && task.Project.Workspace.OwnerId == model.OwnerId);

        if (task is null)
        {
            _logger.LogWarning("Attempt to delete non-existent project task: {ProjectTaskId}", model.ProjectTaskId);
            throw new NotFoundException("Task not found.");
        }

        _context.ProjectTasks.Remove(task);
        _logger.LogInformation("Project task deleted successfully with ID: {ProjectTaskId}", task.Id);

        await _context.SaveChangesAsync();
    }
}