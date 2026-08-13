using Jira.Application.Authentication.Interfaces.Repositories;
using Jira.Domain.Entities;
using Jira.Infrastructure.Persistence;

using Microsoft.EntityFrameworkCore;

namespace Jira.Infrastructure.Authentication;

public sealed class RefreshTokenRepository(AppDbContext context) : IRefreshTokenRepository
{
    private readonly AppDbContext _context = context;

    public async Task<RefreshToken?> GetByTokenHashAsync(string tokenHash, CancellationToken cancellationToken)
    {
        return await _context.RefreshTokens.FirstOrDefaultAsync(refreshToken => refreshToken.TokenHash == tokenHash, cancellationToken).ConfigureAwait(false);
    }

    public async Task AddAsync(RefreshToken refreshToken, CancellationToken cancellationToken)
    {
        await _context.RefreshTokens.AddAsync(refreshToken, cancellationToken).ConfigureAwait(false);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }
}
