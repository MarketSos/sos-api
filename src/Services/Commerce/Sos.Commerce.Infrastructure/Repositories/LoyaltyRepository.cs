using Microsoft.EntityFrameworkCore;
using Sos.Commerce.Application.Interfaces;
using Sos.Commerce.Domain.Entities;
using Sos.Commerce.Infrastructure.Persistence;

namespace Sos.Commerce.Infrastructure.Repositories;

public class LoyaltyRepository(CommerceDbContext db) : ILoyaltyRepository
{
    public Task<LoyaltyAccount?> GetByCustomerIdAsync(Guid customerId, CancellationToken ct = default)
        => db.LoyaltyAccounts.AsTracking()
             .Include(a => a.Transactions)
             .FirstOrDefaultAsync(a => a.CustomerId == customerId, ct);

    public Task<LoyaltyAccount?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => db.LoyaltyAccounts.AsTracking()
             .Include(a => a.Transactions)
             .FirstOrDefaultAsync(a => a.Id == id, ct);

    public async Task AddAsync(LoyaltyAccount account, CancellationToken ct = default)
        => await db.LoyaltyAccounts.AddAsync(account, ct);

    public Task SaveChangesAsync(CancellationToken ct = default)
        => db.SaveChangesAsync(ct);
}
