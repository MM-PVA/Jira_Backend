using Jira.Application.Projects.DTOs;
using Jira.Application.Projects.Interfaces;
using Jira.Application.Projects.Interfaces.Repositories;
using Jira.Domain.Entities;
using Jira.Domain.Exceptions;

using Microsoft.Extensions.Logging;

namespace Jira.Infrastructure.Projects;

public class ProjectService(IProjectRepository projectRepository, ILogger<ProjectService> logger) : IProjectService
{
    private readonly IProjectRepository _projectRepository = projectRepository;
    private readonly ILogger<ProjectService> _logger = logger;

    public async Task<ProjectResponse> CreateAsync(Guid workspaceId, Guid ownerId, CreateProjectRequest request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        try
        {
            var workspaceExists = await _projectRepository.WorkspaceExistsAsync(workspaceId, ownerId, cancellationToken).ConfigureAwait(false);

            if (!workspaceExists)
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

            await _projectRepository.AddAsync(project, cancellationToken).ConfigureAwait(false);

            await _projectRepository.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

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
            var workspaceExists = await _projectRepository.WorkspaceExistsAsync(workspaceId, ownerId, cancellationToken).ConfigureAwait(false);

            if (!workspaceExists)
            {
                _logger.LogWarning("Attempt to get projects for non-existent workspace: {WorkspaceId}", workspaceId);
                throw new NotFoundException("Workspace not found.");
            }

            var projects = await _projectRepository.GetAllAsync(workspaceId, cancellationToken).ConfigureAwait(false);

            return projects.Select(project => new ProjectResponse
            {
                Id = project.Id,
                Name = project.Name,
                Description = project.Description,
                Status = project.Status,
                WorkspaceId = project.WorkspaceId
            });
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
            var workspaceExists = await _projectRepository.WorkspaceExistsAsync(workspaceId, ownerId, cancellationToken).ConfigureAwait(false);

            if (!workspaceExists)
            {
                _logger.LogWarning("Attempt to get project from non-existent workspace: {WorkspaceId}", workspaceId);
                throw new NotFoundException("Workspace not found.");
            }

            var project = await _projectRepository.GetByIdAsync(projectId, workspaceId, cancellationToken).ConfigureAwait(false);

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
            var workspaceExists = await _projectRepository.WorkspaceExistsAsync(workspaceId, ownerId, cancellationToken).ConfigureAwait(false);

            if (!workspaceExists)
            {
                _logger.LogWarning("Attempt to update project in non-existent workspace: {WorkspaceId}", workspaceId);
                throw new NotFoundException("Workspace not found.");
            }

            var project = await _projectRepository.GetByIdAsync(projectId, workspaceId, cancellationToken).ConfigureAwait(false);

            if (project is null)
            {
                _logger.LogWarning("Attempt to update non-existent project: {ProjectId}", projectId);
                throw new NotFoundException("Project not found.");
            }

            project.Name = request.Name;
            project.Description = request.Description;
            project.Status = request.Status;
            project.UpdatedAt = DateTime.UtcNow;

            await _projectRepository.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

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
            var workspaceExists = await _projectRepository.WorkspaceExistsAsync(workspaceId, ownerId, cancellationToken).ConfigureAwait(false);

            if (!workspaceExists)
            {
                _logger.LogWarning("Attempt to delete project from non-existent workspace: {WorkspaceId}", workspaceId);
                throw new NotFoundException("Workspace not found.");
            }

            var project = await _projectRepository.GetByIdAsync(projectId, workspaceId, cancellationToken).ConfigureAwait(false);

            if (project is null)
            {
                _logger.LogWarning("Attempt to delete non-existent project: {ProjectId}", projectId);
                throw new NotFoundException("Project not found.");
            }

            _projectRepository.Remove(project);

            await _projectRepository.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            _logger.LogInformation("Project deleted successfully with ID: {ProjectId}", project.Id);
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("Delete project request cancelled. ProjectId: {ProjectId}", projectId);
            throw;
        }
    }
}
