using Microsoft.EntityFrameworkCore;
using Sos.Pricing.Application.Interfaces;
using Sos.Pricing.Domain.Entities;
using Sos.Pricing.Infrastructure.Persistence;

namespace Sos.Pricing.Infrastructure.Repositories;

public class PriceRuleRepository(PricingDbContext db) : IPriceRuleRepository
{
    public Task<PriceRule?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => db.PriceRules.FirstOrDefaultAsync(x => x.Id == id, ct);

    public async Task<List<PriceRule>> GetActiveRulesForProductAsync(
        Guid productId, Guid? storeId, CancellationToken ct = default)
    {
        var now = DateTimeOffset.UtcNow;
        return await db.PriceRules
            .Where(r => r.ProductId == productId
                     && r.IsActive
                     && r.StartsAt <= now
                     && (r.EndsAt == null || r.EndsAt > now)
                     && (r.StoreId == null || r.StoreId == storeId))
            .ToListAsync(ct);
    }

    public async Task AddAsync(PriceRule rule, CancellationToken ct = default)
        => await db.PriceRules.AddAsync(rule, ct);

    public Task SaveChangesAsync(CancellationToken ct = default)
        => db.SaveChangesAsync(ct);
}
