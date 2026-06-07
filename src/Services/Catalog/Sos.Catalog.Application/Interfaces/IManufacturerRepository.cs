using Sos.Catalog.Domain.Entities;

namespace Sos.Catalog.Application.Interfaces;

public interface IManufacturerRepository
{
    Task<Manufacturer?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<Manufacturer?> GetByCodeAsync(string code, CancellationToken ct = default);
    Task<List<Manufacturer>> GetAllAsync(CancellationToken ct = default);
    Task<bool> CodeExistsAsync(string code, CancellationToken ct = default);
    Task AddAsync(Manufacturer manufacturer, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}
