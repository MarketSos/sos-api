using Sos.POS.Domain.Entities;

namespace Sos.POS.Application.Interfaces;

public interface ISaleRepository
{
    Task<Sale?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IEnumerable<Sale>> GetByStoreAsync(Guid storeId, DateTime from, DateTime to, CancellationToken ct = default);
    Task AddAsync(Sale sale, CancellationToken ct = default);
    Task UpdateAsync(Sale sale, CancellationToken ct = default);
}
