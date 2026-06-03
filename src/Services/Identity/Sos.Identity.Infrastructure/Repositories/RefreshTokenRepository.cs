using Microsoft.EntityFrameworkCore;
using Sos.Identity.Application.Interfaces;
using Sos.Identity.Domain.Entities;
using Sos.Identity.Infrastructure.Persistence;

namespace Sos.Identity.Infrastructure.Repositories;

public class RefreshTokenRepository(IdentityDbContext db) : IRefreshTokenRepository
{
    public Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken ct = default)
        => db.RefreshTokens.FirstOrDefaultAsync(t => t.Token == token, ct);

    public async Task AddAsync(RefreshToken token, CancellationToken ct = default)
    {
        await db.RefreshTokens.AddAsync(token, ct);
        await db.SaveChangesAsync(ct);
    }

    public async Task RevokeAsync(string token, CancellationToken ct = default)
    {
        var existing = await db.RefreshTokens.FirstOrDefaultAsync(t => t.Token == token, ct);
        if (existing is not null)
        {
            existing.Revoke();
            await db.SaveChangesAsync(ct);
        }
    }
}
