using Microsoft.EntityFrameworkCore;
using Sos.Core.Application.Interfaces;
using Sos.Core.Domain.Entities;
using Sos.Core.Infrastructure.Persistence;

namespace Sos.Core.Infrastructure.Repositories;

public class EmployeeRankRepository(CoreDbContext db) : IEmployeeRankRepository
{
    public Task<EmployeeRank?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => db.EmployeeRanks.AsTracking().FirstOrDefaultAsync(r => r.Id == id, ct);

    public Task<EmployeeRank?> GetByCodeAsync(string code, CancellationToken ct = default)
        => db.EmployeeRanks.AsTracking().FirstOrDefaultAsync(
            r => r.Code == code.ToUpperInvariant(), ct);

    public Task<List<EmployeeRank>> GetAllAsync(CancellationToken ct = default)
        => db.EmployeeRanks.OrderBy(r => r.Code).ToListAsync(ct);

    public Task<bool> CodeExistsAsync(string code, CancellationToken ct = default)
        => db.EmployeeRanks.AnyAsync(r => r.Code == code.ToUpperInvariant(), ct);

    public async Task AddAsync(EmployeeRank rank, CancellationToken ct = default)
    {
        await db.EmployeeRanks.AddAsync(rank, ct);
        await db.SaveChangesAsync(ct);
    }

    public Task SaveChangesAsync(CancellationToken ct = default)
        => db.SaveChangesAsync(ct);
}
