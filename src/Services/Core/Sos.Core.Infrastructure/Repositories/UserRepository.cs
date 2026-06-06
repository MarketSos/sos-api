using Microsoft.EntityFrameworkCore;
using Sos.Core.Application.Interfaces;
using Sos.Core.Domain.Entities;
using Sos.Core.Infrastructure.Persistence;

namespace Sos.Core.Infrastructure.Repositories;

public class UserRepository(CoreDbContext db) : IUserRepository
{
    public Task<User?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => db.Users.AsTracking().FirstOrDefaultAsync(u => u.Id == id, ct);

    public Task<User?> GetByEmailAsync(string email, CancellationToken ct = default)
        => db.Users.AsTracking().FirstOrDefaultAsync(u => u.Email == email.ToLowerInvariant(), ct);

    public Task<bool> ExistsAsync(string email, CancellationToken ct = default)
        => db.Users.AnyAsync(u => u.Email == email.ToLowerInvariant(), ct);

    public async Task AddAsync(User user, CancellationToken ct = default)
    {
        await db.Users.AddAsync(user, ct);
        await db.SaveChangesAsync(ct);
    }

    public Task SaveChangesAsync(CancellationToken ct = default)
        => db.SaveChangesAsync(ct);
}
