using Jira.Application.Authentication.Interfaces.Repositories;
using Jira.Domain.Entities;
using Jira.Infrastructure.Persistence;

using Microsoft.EntityFrameworkCore;

namespace Jira.Infrastructure.Authentication;

public sealed class UserRepository(AppDbContext context) : IUserRepository
{
    private readonly AppDbContext _context = context;

    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken)
    {
        return await _context.Users.FirstOrDefaultAsync(user => user.Email == email, cancellationToken).ConfigureAwait(false);
    }

    public async Task<User?> GetByIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        return await _context.Users.FirstOrDefaultAsync(user => user.Id == userId, cancellationToken).ConfigureAwait(false);
    }

    public async Task AddAsync(User user, CancellationToken cancellationToken)
    {
        await _context.Users.AddAsync(user, cancellationToken).ConfigureAwait(false);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }
}
