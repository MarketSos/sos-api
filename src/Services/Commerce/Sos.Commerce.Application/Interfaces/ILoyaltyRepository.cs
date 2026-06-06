using Sos.Commerce.Domain.Entities;

namespace Sos.Commerce.Application.Interfaces;

public interface ILoyaltyRepository
{
    Task<LoyaltyAccount?> GetByCustomerIdAsync(Guid customerId, CancellationToken ct = default);
    Task<LoyaltyAccount?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task                  AddAsync(LoyaltyAccount account, CancellationToken ct = default);
    Task                  SaveChangesAsync(CancellationToken ct = default);
}
