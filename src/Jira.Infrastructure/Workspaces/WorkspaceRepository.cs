using Jira.Application.Workspaces.Interfaces.Repositories;
using Jira.Domain.Entities;
using Jira.Infrastructure.Persistence;

using Microsoft.EntityFrameworkCore;

namespace Jira.Infrastructure.Workspaces;

public sealed class WorkspaceRepository(AppDbContext context) : IWorkspaceRepository
{
    private readonly AppDbContext _context = context;

    public async Task AddAsync(Workspace workspace, CancellationToken cancellationToken)
    {
        await _context.Workspaces.AddAsync(workspace, cancellationToken).ConfigureAwait(false);
    }

    public async Task<IEnumerable<Workspace>> GetAllAsync(Guid ownerId, CancellationToken cancellationToken)
    {
        return await _context.Workspaces.Where(workspace => workspace.OwnerId == ownerId).ToListAsync(cancellationToken).ConfigureAwait(false);
    }

    public async Task<Workspace?> GetByIdAsync(Guid workspaceId, Guid ownerId, CancellationToken cancellationToken)
    {
        return await _context.Workspaces.FirstOrDefaultAsync(workspace => workspace.Id == workspaceId && workspace.OwnerId == ownerId, cancellationToken).ConfigureAwait(false);
    }

    public void Remove(Workspace workspace)
    {
        _context.Workspaces.Remove(workspace);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }
}
