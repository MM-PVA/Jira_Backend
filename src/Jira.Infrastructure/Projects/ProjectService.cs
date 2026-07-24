using Jira.Application.Projects.DTOs;
using Jira.Application.Projects.Interfaces;
using Jira.Domain.Entities;
using Jira.Domain.Exceptions;
using Jira.Infrastructure.Persistence;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Jira.Infrastructure.Projects;

public class ProjectService(AppDbContext context, ILogger<ProjectService> logger) : IProjectService
{
    private readonly AppDbContext _context = context;
    private readonly ILogger<ProjectService> _logger = logger;

    public async Task<ProjectResponse> CreateAsync(Guid workspaceId, Guid ownerId, CreateProjectRequest request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        try
        {
            var workspace = await _context.Workspaces.FirstOrDefaultAsync(workspace => workspace.Id == workspaceId && workspace.OwnerId == ownerId, cancellationToken).ConfigureAwait(false);

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

            await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            return new ProjectResponse
            {
                Id = project.Id,
                Name = project.Name,
                Description = project.Description,
                Status = project.Status,
                WorkspaceId = project.WorkspaceId
            };
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("Create project request cancelled. WorkspaceId: {WorkspaceId}, OwnerId: {OwnerId}", workspaceId, ownerId);
            throw;
        }
    }

    public async Task<IEnumerable<ProjectResponse>> GetAllAsync(Guid workspaceId, Guid ownerId, CancellationToken cancellationToken)
    {
        try
        {
            var workspace = await _context.Workspaces.FirstOrDefaultAsync(workspace => workspace.Id == workspaceId && workspace.OwnerId == ownerId, cancellationToken).ConfigureAwait(false);

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
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("Get all projects request cancelled. WorkspaceId: {WorkspaceId}, OwnerId: {OwnerId}", workspaceId, ownerId);
            throw;
        }
    }

    public async Task<ProjectResponse> GetByIdAsync(Guid projectId, Guid workspaceId, Guid ownerId, CancellationToken cancellationToken)
    {
        try
        {
            var project = await _context.Projects.Include(project => project.Workspace).FirstOrDefaultAsync(project => project.Id == projectId && project.WorkspaceId == workspaceId && project.Workspace.OwnerId == ownerId, cancellationToken).ConfigureAwait(false);

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
        catch (OperationCanceledException)
        {
            _logger.LogWarning("Get project request cancelled. ProjectId: {ProjectId}", projectId);
            throw;
        }
    }

    public async Task<ProjectResponse> UpdateAsync(Guid projectId, Guid workspaceId, Guid ownerId, UpdateProjectRequest request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        try
        {
            var project = await _context.Projects.Include(project => project.Workspace).FirstOrDefaultAsync(project => project.Id == projectId && project.WorkspaceId == workspaceId && project.Workspace.OwnerId == ownerId, cancellationToken).ConfigureAwait(false);

            if (project is null)
            {
                _logger.LogWarning("Attempt to update non-existent project: {ProjectId}", projectId);
                throw new NotFoundException("Project not found.");
            }

            project.Name = request.Name;
            project.Description = request.Description;
            project.Status = request.Status;
            project.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

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
        catch (OperationCanceledException)
        {
            _logger.LogWarning("Update project request cancelled. ProjectId: {ProjectId}", projectId);
            throw;
        }
    }

    public async Task DeleteAsync(Guid projectId, Guid workspaceId, Guid ownerId, CancellationToken cancellationToken)
    {
        try
        {
            var project = await _context.Projects.Include(project => project.Workspace).FirstOrDefaultAsync(project => project.Id == projectId && project.WorkspaceId == workspaceId && project.Workspace.OwnerId == ownerId, cancellationToken).ConfigureAwait(false);

            if (project is null)
            {
                _logger.LogWarning("Attempt to delete non-existent project: {ProjectId}", projectId);
                throw new NotFoundException("Project not found.");
            }

            _context.Projects.Remove(project);

            _logger.LogInformation("Project deleted successfully with ID: {ProjectId}", project.Id);

            await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("Delete project request cancelled. ProjectId: {ProjectId}", projectId);
            throw;
        }
    }
}
