using Sos.Catalog.Domain.Entities;

namespace Sos.Catalog.Application.Interfaces;

public interface IBrandRepository
{
    Task<Brand?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<Brand?> GetByCodeAsync(string code, CancellationToken ct = default);
    Task<List<Brand>> GetAllAsync(CancellationToken ct = default);
    Task<bool> CodeExistsAsync(string code, CancellationToken ct = default);
    Task AddAsync(Brand brand, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}
