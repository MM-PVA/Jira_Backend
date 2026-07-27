using Jira.Domain.Entities;

namespace Jira.Application.Workspaces.Interfaces.Repositories;

public interface IWorkspaceRepository
{
    Task AddAsync(Workspace workspace, CancellationToken cancellationToken);

    Task<IEnumerable<Workspace>> GetAllAsync(Guid ownerId, CancellationToken cancellationToken);

    Task<Workspace?> GetByIdAsync(Guid workspaceId, Guid ownerId, CancellationToken cancellationToken);

    void Remove(Workspace workspace);

    Task SaveChangesAsync(CancellationToken cancellationToken);
}
