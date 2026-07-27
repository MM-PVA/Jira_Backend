using Jira.Application.ProjectTasks.Interfaces.Repositories;
using Jira.Domain.Entities;
using Jira.Infrastructure.Persistence;

using Microsoft.EntityFrameworkCore;

namespace Jira.Infrastructure.ProjectTasks;

public sealed class ProjectTaskRepository(AppDbContext context) : IProjectTaskRepository
{
    private readonly AppDbContext _context = context;

    public async Task<Project?> GetProjectAsync(Guid projectId, Guid workspaceId, Guid ownerId, CancellationToken cancellationToken)
    {
        return await _context.Projects.Include(project => project.Workspace).FirstOrDefaultAsync(project => project.Id == projectId && project.WorkspaceId == workspaceId && project.Workspace.OwnerId == ownerId, cancellationToken).ConfigureAwait(false);
    }

    public async Task AddAsync(ProjectTask projectTask, CancellationToken cancellationToken)
    {
        await _context.ProjectTasks.AddAsync(projectTask, cancellationToken).ConfigureAwait(false);
    }

    public async Task<IEnumerable<ProjectTask>> GetAllAsync(Guid projectId, Guid workspaceId, Guid ownerId, string? search, CancellationToken cancellationToken)
    {
        var query = _context.ProjectTasks.Where(task => task.ProjectId == projectId && task.Project.WorkspaceId == workspaceId && task.Project.Workspace.OwnerId == ownerId);

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(task => EF.Functions.Like(task.Title, $"%{search}%"));
        }

        return await query.ToListAsync(cancellationToken).ConfigureAwait(false);
    }

    public async Task<ProjectTask?> GetByIdAsync(Guid projectTaskId, Guid projectId, Guid workspaceId, Guid ownerId, CancellationToken cancellationToken)
    {
        return await _context.ProjectTasks.FirstOrDefaultAsync(task => task.Id == projectTaskId && task.ProjectId == projectId && task.Project.WorkspaceId == workspaceId && task.Project.Workspace.OwnerId == ownerId, cancellationToken).ConfigureAwait(false);
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
