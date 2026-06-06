using Sos.Catalog.Domain.Entities;

namespace Sos.Catalog.Application.Interfaces;

public interface IPriceRuleRepository
{
    Task<PriceRule?>      GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<List<PriceRule>> GetActiveRulesForProductAsync(Guid productId, Guid? storeId, CancellationToken ct = default);
    Task                  AddAsync(PriceRule rule, CancellationToken ct = default);
    Task                  SaveChangesAsync(CancellationToken ct = default);
}
