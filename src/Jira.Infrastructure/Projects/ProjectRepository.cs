using Jira.Application.Projects.Interfaces.Repositories;
using Jira.Domain.Entities;
using Jira.Infrastructure.Persistence;

using Microsoft.EntityFrameworkCore;

namespace Jira.Infrastructure.Projects;

public sealed class ProjectRepository(AppDbContext context) : IProjectRepository
{
    private readonly AppDbContext _context = context;

    public async Task<bool> WorkspaceExistsAsync(Guid workspaceId, Guid ownerId, CancellationToken cancellationToken)
    {
        return await _context.Workspaces.AnyAsync(workspace => workspace.Id == workspaceId && workspace.OwnerId == ownerId, cancellationToken).ConfigureAwait(false);
    }

    public async Task AddAsync(Project project, CancellationToken cancellationToken)
    {
        await _context.Projects.AddAsync(project, cancellationToken).ConfigureAwait(false);
    }

    public async Task<IEnumerable<Project>> GetAllAsync(Guid workspaceId, CancellationToken cancellationToken)
    {
        return await _context.Projects.Where(project => project.WorkspaceId == workspaceId).ToListAsync(cancellationToken).ConfigureAwait(false);
    }

    public async Task<Project?> GetByIdAsync(Guid projectId, Guid workspaceId, Guid ownerId, CancellationToken cancellationToken)
    {
        return await _context.Projects.Include(project => project.Workspace).FirstOrDefaultAsync(project => project.Id == projectId && project.WorkspaceId == workspaceId && project.Workspace.OwnerId == ownerId, cancellationToken).ConfigureAwait(false);
    }

    public void Remove(Project project)
    {
        _context.Projects.Remove(project);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }
}
