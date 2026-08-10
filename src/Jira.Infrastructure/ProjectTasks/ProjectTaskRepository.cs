using Jira.Application.ProjectTasks.Interfaces.Repositories;
using Jira.Domain.Entities;
using Jira.Infrastructure.Persistence;

using Microsoft.EntityFrameworkCore;

namespace Jira.Infrastructure.ProjectTasks;

public sealed class ProjectTaskRepository(AppDbContext context) : IProjectTaskRepository
{
    private readonly AppDbContext _context = context;

    public async Task<bool> ProjectExistsAsync(Guid projectId, Guid workspaceId, Guid ownerId, CancellationToken cancellationToken)
    {
        var workspace = await _context.Workspaces.FirstOrDefaultAsync(workspace => workspace.Id == workspaceId && workspace.OwnerId == ownerId, cancellationToken).ConfigureAwait(false);

        if (workspace is null)
        {
            return false;
        }

        var project = await _context.Projects.FirstOrDefaultAsync(project => project.Id == projectId && project.WorkspaceId == workspaceId, cancellationToken).ConfigureAwait(false);

        return project is not null;
    }

    public async Task AddAsync(ProjectTask projectTask, CancellationToken cancellationToken)
    {
        await _context.ProjectTasks.AddAsync(projectTask, cancellationToken).ConfigureAwait(false);
    }

    public async Task<(IEnumerable<ProjectTask> Items, int TotalCount)> GetAllAsync(
        Guid projectId,
        string? search,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken)
    {
        var query = _context.ProjectTasks
            .Where(task => task.ProjectId == projectId);

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(task =>
                EF.Functions.Like(task.Title, $"%{search}%"));
        }

        var totalCount = await query
            .CountAsync(cancellationToken)
            .ConfigureAwait(false);

        var items = await query
            .OrderBy(task => task.Title)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        return (items, totalCount);
    }

    public async Task<ProjectTask?> GetByIdAsync(Guid projectTaskId, Guid projectId, CancellationToken cancellationToken)
    {
        return await _context.ProjectTasks.FirstOrDefaultAsync(task => task.Id == projectTaskId && task.ProjectId == projectId, cancellationToken).ConfigureAwait(false);
    }

    public void Remove(ProjectTask projectTask)
    {
        _context.ProjectTasks.Remove(projectTask);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }
}
