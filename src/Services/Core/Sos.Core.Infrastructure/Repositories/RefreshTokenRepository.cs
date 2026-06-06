using Microsoft.EntityFrameworkCore;
using Sos.Core.Application.Interfaces;
using Sos.Core.Domain.Entities;
using Sos.Core.Infrastructure.Persistence;

namespace Sos.Core.Infrastructure.Repositories;

public class RefreshTokenRepository(CoreDbContext db) : IRefreshTokenRepository
{
    public Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken ct = default)
        => db.RefreshTokens.AsTracking().FirstOrDefaultAsync(t => t.Token == token, ct);

    public async Task AddAsync(RefreshToken token, CancellationToken ct = default)
    {
        await db.RefreshTokens.AddAsync(token, ct);
        await db.SaveChangesAsync(ct);
    }

    public async Task RevokeAsync(string token, CancellationToken ct = default)
    {
        var existing = await db.RefreshTokens.AsTracking()
                               .FirstOrDefaultAsync(t => t.Token == token, ct);
        if (existing is not null)
        {
            existing.Revoke();
            await db.SaveChangesAsync(ct);
        }
    }
}
