using Microsoft.EntityFrameworkCore;
using Sos.Core.Application.Interfaces;
using Sos.Core.Domain.Entities;
using Sos.Core.Infrastructure.Persistence;

namespace Sos.Core.Infrastructure.Repositories;

public class SpecializationRepository(CoreDbContext db) : ISpecializationRepository
{
    public Task<Specialization?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => db.Specializations.AsTracking().FirstOrDefaultAsync(s => s.Id == id, ct);

    public Task<Specialization?> GetByCodeAsync(string code, CancellationToken ct = default)
        => db.Specializations.AsTracking().FirstOrDefaultAsync(
            s => s.Code == code.ToUpperInvariant(), ct);

    public Task<List<Specialization>> GetAllAsync(CancellationToken ct = default)
        => db.Specializations.OrderBy(s => s.Code).ToListAsync(ct);

    public Task<bool> CodeExistsAsync(string code, CancellationToken ct = default)
        => db.Specializations.AnyAsync(s => s.Code == code.ToUpperInvariant(), ct);

    public async Task AddAsync(Specialization specialization, CancellationToken ct = default)
    {
        await db.Specializations.AddAsync(specialization, ct);
        await db.SaveChangesAsync(ct);
    }

    public Task SaveChangesAsync(CancellationToken ct = default)
        => db.SaveChangesAsync(ct);
}
