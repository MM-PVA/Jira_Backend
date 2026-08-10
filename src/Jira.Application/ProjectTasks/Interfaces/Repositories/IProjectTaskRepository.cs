using Jira.Domain.Entities;

namespace Jira.Application.ProjectTasks.Interfaces.Repositories;

public interface IProjectTaskRepository
{
    Task<bool> ProjectExistsAsync(Guid projectId, Guid workspaceId, Guid ownerId, CancellationToken cancellationToken);

    Task AddAsync(ProjectTask projectTask, CancellationToken cancellationToken);

    Task<(IEnumerable<ProjectTask> Items, int TotalCount)> GetAllAsync(Guid projectId, string? search, int pageNumber, int pageSize, CancellationToken cancellationToken);

    Task<ProjectTask?> GetByIdAsync(Guid projectTaskId, Guid projectId, CancellationToken cancellationToken);

    void Remove(ProjectTask projectTask);

    Task SaveChangesAsync(CancellationToken cancellationToken);
}
