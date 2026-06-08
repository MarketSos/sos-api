using Sos.Catalog.Domain.Entities;

namespace Sos.Catalog.Application.Interfaces;

public interface IStoreRepository
{
    Task<Store?>       GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<Store?>       GetByCodeAsync(Guid organizationId, string code, CancellationToken ct = default);
    Task<List<Store>>  GetByOrganizationAsync(Guid organizationId, CancellationToken ct = default);
    Task<bool>         CodeExistsAsync(Guid organizationId, string code, CancellationToken ct = default);
    Task               AddAsync(Store store, CancellationToken ct = default);
    Task               SaveChangesAsync(CancellationToken ct = default);
}
