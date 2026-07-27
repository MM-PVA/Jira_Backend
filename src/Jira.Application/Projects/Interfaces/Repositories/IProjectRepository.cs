using Jira.Domain.Entities;

namespace Jira.Application.Projects.Interfaces.Repositories;

public interface IProjectRepository
{
    Task<bool> WorkspaceExistsAsync(Guid workspaceId, Guid ownerId, CancellationToken cancellationToken);

    Task AddAsync(Project project, CancellationToken cancellationToken);

    Task<IEnumerable<Project>> GetAllAsync(Guid workspaceId, CancellationToken cancellationToken);

    Task<Project?> GetByIdAsync(Guid projectId, Guid workspaceId, Guid ownerId, CancellationToken cancellationToken);

    Task SaveChangesAsync(CancellationToken cancellationToken);

    void Remove(Project project);
}
