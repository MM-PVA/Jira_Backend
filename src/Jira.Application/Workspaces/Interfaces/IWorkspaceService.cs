using Jira.Application.Workspaces.DTOs;

namespace Jira.Application.Workspaces.Interfaces;

public interface IWorkspaceService
{
    Task<WorkspaceResponse> CreateAsync(Guid ownerId, CreateWorkspaceRequest request, CancellationToken cancellationToken);

    Task<IEnumerable<WorkspaceResponse>> GetAllAsync(Guid ownerId, CancellationToken cancellationToken);

    Task<WorkspaceResponse> GetByIdAsync(Guid workspaceId, Guid ownerId, CancellationToken cancellationToken);

    Task<WorkspaceResponse> UpdateAsync(Guid workspaceId, Guid ownerId, UpdateWorkspaceRequest request, CancellationToken cancellationToken);

    Task DeleteAsync(Guid workspaceId, Guid ownerId, CancellationToken cancellationToken);
}
