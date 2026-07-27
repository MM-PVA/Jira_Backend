using Jira.Domain.Entities;

namespace Jira.Application.ProjectTasks.Interfaces.Repositories;

public interface IProjectTaskRepository
{
    Task<Project?> GetProjectAsync(Guid projectId, Guid workspaceId, Guid ownerId, CancellationToken cancellationToken);

    Task AddAsync(ProjectTask projectTask, CancellationToken cancellationToken);

    Task<IEnumerable<ProjectTask>> GetAllAsync(Guid projectId, Guid workspaceId, Guid ownerId, string? search, CancellationToken cancellationToken);

    Task<ProjectTask?> GetByIdAsync(Guid projectTaskId, Guid projectId, Guid workspaceId, Guid ownerId, CancellationToken cancellationToken);

    void Remove(ProjectTask projectTask);

    Task SaveChangesAsync(CancellationToken cancellationToken);
}
