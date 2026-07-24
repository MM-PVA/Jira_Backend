using Jira.Application.Projects.DTOs;

namespace Jira.Application.Projects.Interfaces;

public interface IProjectService
{
    Task<ProjectResponse> CreateAsync(Guid workspaceId, Guid ownerId, CreateProjectRequest request, CancellationToken cancellationToken);

    Task<IEnumerable<ProjectResponse>> GetAllAsync(Guid workspaceId, Guid ownerId, CancellationToken cancellationToken);

    Task<ProjectResponse> GetByIdAsync(Guid projectId, Guid workspaceId, Guid ownerId, CancellationToken cancellationToken);

    Task<ProjectResponse> UpdateAsync(Guid projectId, Guid workspaceId, Guid ownerId, UpdateProjectRequest request, CancellationToken cancellationToken);

    Task DeleteAsync(Guid projectId, Guid workspaceId, Guid ownerId, CancellationToken cancellationToken);
}
