using Jira.Application.Projects.DTOs;

namespace Jira.Application.Projects.Interfaces;

public interface IProjectService
{
    Task<ProjectResponse> CreateAsync(Guid workspaceId, Guid ownerId, CreateProjectRequest request);

    Task<IEnumerable<ProjectResponse>> GetAllAsync(Guid workspaceId, Guid ownerId);

    Task<ProjectResponse> GetByIdAsync(Guid projectId, Guid workspaceId, Guid ownerId);

    Task<ProjectResponse> UpdateAsync(Guid projectId, Guid workspaceId, Guid ownerId, UpdateProjectRequest request);

    Task DeleteAsync(Guid projectId, Guid workspaceId, Guid ownerId);
}
