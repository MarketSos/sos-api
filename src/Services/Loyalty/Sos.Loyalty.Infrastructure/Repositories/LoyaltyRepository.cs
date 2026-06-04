using Microsoft.EntityFrameworkCore;
using Sos.Loyalty.Application.Interfaces;
using Sos.Loyalty.Domain.Entities;
using Sos.Loyalty.Infrastructure.Persistence;

namespace Sos.Loyalty.Infrastructure.Repositories;

public class LoyaltyRepository(LoyaltyDbContext db) : ILoyaltyRepository
{
    public Task<LoyaltyAccount?> GetByCustomerIdAsync(Guid customerId, CancellationToken ct = default)
        => db.Accounts
             .Include(a => a.Transactions)
             .FirstOrDefaultAsync(a => a.CustomerId == customerId, ct);

    public Task<LoyaltyAccount?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => db.Accounts
             .Include(a => a.Transactions)
             .FirstOrDefaultAsync(a => a.Id == id, ct);

    public async Task AddAsync(LoyaltyAccount account, CancellationToken ct = default)
        => await db.Accounts.AddAsync(account, ct);

    public Task SaveChangesAsync(CancellationToken ct = default)
        => db.SaveChangesAsync(ct);
}
