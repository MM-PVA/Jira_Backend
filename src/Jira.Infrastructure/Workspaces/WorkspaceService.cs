using Jira.Application.Workspaces.DTOs;
using Jira.Application.Workspaces.Interfaces;
using Jira.Domain.Entities;
using Jira.Domain.Exceptions;
using Jira.Infrastructure.Persistence;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Jira.Infrastructure.Workspaces;

public class WorkspaceService(AppDbContext context, ILogger<WorkspaceService> logger) : IWorkspaceService
{
    private readonly AppDbContext _context = context;
    private readonly ILogger<WorkspaceService> _logger = logger;

    public async Task<WorkspaceResponse> CreateAsync(Guid ownerId, CreateWorkspaceRequest request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        try
        {
            var workspace = new Workspace
            {
                Name = request.Name,
                Description = request.Description,
                OwnerId = ownerId,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Workspaces.Add(workspace);

            await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            _logger.LogInformation("Workspace created successfully with ID: {WorkspaceId}", workspace.Id);

            return new WorkspaceResponse
            {
                Id = workspace.Id,
                Name = workspace.Name,
                Description = workspace.Description,
                OwnerId = workspace.OwnerId
            };
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("Create workspace request cancelled. OwnerId: {OwnerId}", ownerId);
            throw;
        }
    }

    public async Task<IEnumerable<WorkspaceResponse>> GetAllAsync(Guid ownerId, CancellationToken cancellationToken)
    {
        try
        {
            return await _context.Workspaces.Where(workspace => workspace.OwnerId == ownerId)
                .Select(workspace => new WorkspaceResponse
                {
                    Id = workspace.Id,
                    Name = workspace.Name,
                    Description = workspace.Description,
                    OwnerId = workspace.OwnerId
                })
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("Get all workspaces request cancelled. OwnerId: {OwnerId}", ownerId);
            throw;
        }
    }

    public async Task<WorkspaceResponse> GetByIdAsync(Guid workspaceId, Guid ownerId, CancellationToken cancellationToken)
    {
        try
        {
            var workspace = await _context.Workspaces.FirstOrDefaultAsync(workspace => workspace.Id == workspaceId && workspace.OwnerId == ownerId, cancellationToken).ConfigureAwait(false);

            if (workspace is null)
            {
                _logger.LogWarning("Attempt to get non-existent workspace: {WorkspaceId}", workspaceId);
                throw new NotFoundException("Workspace not found.");
            }

            _logger.LogInformation("Workspace retrieved successfully with ID: {WorkspaceId}", workspace.Id);

            return new WorkspaceResponse
            {
                Id = workspace.Id,
                Name = workspace.Name,
                Description = workspace.Description,
                OwnerId = workspace.OwnerId
            };
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("Get workspace request cancelled. WorkspaceId: {WorkspaceId}", workspaceId);
            throw;
        }
    }

    public async Task<WorkspaceResponse> UpdateAsync(Guid workspaceId, Guid ownerId, UpdateWorkspaceRequest request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        try
        {
            var workspace = await _context.Workspaces.FirstOrDefaultAsync(workspace => workspace.Id == workspaceId && workspace.OwnerId == ownerId, cancellationToken).ConfigureAwait(false);

            if (workspace is null)
            {
                _logger.LogWarning("Attempt to update non-existent workspace: {WorkspaceId}", workspaceId);
                throw new NotFoundException("Workspace not found.");
            }

            workspace.Name = request.Name;
            workspace.Description = request.Description;
            workspace.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            _logger.LogInformation("Workspace updated successfully with ID: {WorkspaceId}", workspace.Id);

            return new WorkspaceResponse
            {
                Id = workspace.Id,
                Name = workspace.Name,
                Description = workspace.Description,
                OwnerId = workspace.OwnerId
            };
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("Update workspace request cancelled. WorkspaceId: {WorkspaceId}", workspaceId);
            throw;
        }
    }

    public async Task DeleteAsync(Guid workspaceId, Guid ownerId, CancellationToken cancellationToken)
    {
        try
        {
            var workspace = await _context.Workspaces.FirstOrDefaultAsync(workspace => workspace.Id == workspaceId && workspace.OwnerId == ownerId, cancellationToken).ConfigureAwait(false);

            if (workspace is null)
            {
                _logger.LogWarning("Attempt to delete non-existent workspace: {WorkspaceId}", workspaceId);
                throw new NotFoundException("Workspace not found.");
            }

            _context.Workspaces.Remove(workspace);

            _logger.LogInformation("Workspace deleted successfully with ID: {WorkspaceId}", workspace.Id);

            await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("Delete workspace request cancelled. WorkspaceId: {WorkspaceId}", workspaceId);
            throw;
        }
    }
}
