using Jira.Application.Projects.DTOs;
using Jira.Application.Projects.Interfaces;
using Jira.Infrastructure.Persistence;
using Jira.Domain.Entities;
using Jira.Domain.Exceptions;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Jira.Infrastructure.Projects;

public class ProjectService(AppDbContext context, ILogger<ProjectService> logger) : IProjectService
{
    private readonly AppDbContext _context = context;
    private readonly ILogger<ProjectService> _logger = logger;

    public async Task<ProjectResponse> CreateAsync(Guid workspaceId, Guid ownerId, CreateProjectRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        var workspace = await _context.Workspaces.FirstOrDefaultAsync(workspace =>
            workspace.Id == workspaceId && workspace.OwnerId == ownerId).ConfigureAwait(false);

        if (workspace is null)
        {
            _logger.LogWarning("Attempt to create project in non-existent workspace: {WorkspaceId}", workspaceId);
            throw new NotFoundException("Workspace not found.");
        }

        var project = new Project
        {
            Name = request.Name,
            Description = request.Description,
            Status = request.Status,
            WorkspaceId = workspaceId,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Projects.Add(project);

        await _context.SaveChangesAsync().ConfigureAwait(false);

        _logger.LogInformation("Project created successfully with ID: {ProjectId}", project.Id);

        return new ProjectResponse
        {
            Id = project.Id,
            Name = project.Name,
            Description = project.Description,
            Status = project.Status,
            WorkspaceId = project.WorkspaceId
        };
    }

    public async Task<IEnumerable<ProjectResponse>> GetAllAsync(Guid workspaceId, Guid ownerId)
    {
        var workspace = await _context.Workspaces.FirstOrDefaultAsync(workspace =>
            workspace.Id == workspaceId && workspace.OwnerId == ownerId).ConfigureAwait(false);

        if (workspace is null)
        {
            _logger.LogWarning("Attempt to get projects for non-existent workspace: {WorkspaceId}", workspaceId);
            throw new NotFoundException("Workspace not found.");
        }

        return await _context.Projects
            .Where(project => project.WorkspaceId == workspaceId)
            .Select(project => new ProjectResponse
            {
                Id = project.Id,
                Name = project.Name,
                Description = project.Description,
                Status = project.Status,
                WorkspaceId = project.WorkspaceId
            })
            .ToListAsync()
            .ConfigureAwait(false);
    }

    public async Task<ProjectResponse> GetByIdAsync(Guid projectId, Guid workspaceId, Guid ownerId)
    {
        var project = await _context.Projects.Include(project => project.Workspace).FirstOrDefaultAsync(project =>
                project.Id == projectId && project.WorkspaceId == workspaceId && project.Workspace.OwnerId == ownerId).ConfigureAwait(false);

        if (project is null)
        {
            _logger.LogWarning("Attempt to get non-existent project: {ProjectId}", projectId);
            throw new NotFoundException("Project not found.");
        }

        return new ProjectResponse
        {
            Id = project.Id,
            Name = project.Name,
            Description = project.Description,
            Status = project.Status,
            WorkspaceId = project.WorkspaceId
        };
    }

    public async Task<ProjectResponse> UpdateAsync(Guid projectId, Guid workspaceId, Guid ownerId, UpdateProjectRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        var project = await _context.Projects.Include(project => project.Workspace).FirstOrDefaultAsync(project =>
                project.Id == projectId && project.WorkspaceId == workspaceId && project.Workspace.OwnerId == ownerId).ConfigureAwait(false);

        if (project is null)
        {
            _logger.LogWarning("Attempt to update non-existent project: {ProjectId}", projectId);
            throw new NotFoundException("Project not found.");
        }

        project.Name = request.Name;
        project.Description = request.Description;
        project.Status = request.Status;
        project.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync().ConfigureAwait(false);

        _logger.LogInformation("Project updated successfully with ID: {ProjectId}", project.Id);

        return new ProjectResponse
        {
            Id = project.Id,
            Name = project.Name,
            Description = project.Description,
            Status = project.Status,
            WorkspaceId = project.WorkspaceId
        };
    }

    public async Task DeleteAsync(Guid projectId, Guid workspaceId, Guid ownerId)
    {
        var project = await _context.Projects.Include(project => project.Workspace).FirstOrDefaultAsync(project =>
                project.Id == projectId && project.WorkspaceId == workspaceId && project.Workspace.OwnerId == ownerId).ConfigureAwait(false);

        if (project is null)
        {
            _logger.LogWarning("Attempt to delete non-existent project: {ProjectId}", projectId);
            throw new NotFoundException("Project not found.");
        }

        _context.Projects.Remove(project);

        _logger.LogInformation("Project deleted successfully with ID: {ProjectId}", project.Id);

        await _context.SaveChangesAsync().ConfigureAwait(false);
    }
}
