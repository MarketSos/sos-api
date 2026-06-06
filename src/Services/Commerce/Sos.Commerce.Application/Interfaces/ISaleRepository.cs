using Sos.Commerce.Domain.Entities;

namespace Sos.Commerce.Application.Interfaces;

public interface ISaleRepository
{
    Task<Sale?>              GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IEnumerable<Sale>> GetByStoreAsync(Guid storeId, DateTime from, DateTime to, CancellationToken ct = default);
    Task                    AddAsync(Sale sale, CancellationToken ct = default);
    Task                    SaveChangesAsync(CancellationToken ct = default);
}
