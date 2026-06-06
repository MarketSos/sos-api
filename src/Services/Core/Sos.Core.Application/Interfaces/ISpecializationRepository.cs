using Sos.Core.Domain.Entities;

namespace Sos.Core.Application.Interfaces;

public interface ISpecializationRepository
{
    Task<Specialization?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<Specialization?> GetByCodeAsync(string code, CancellationToken ct = default);
    Task<List<Specialization>> GetAllAsync(CancellationToken ct = default);
    Task<bool> CodeExistsAsync(string code, CancellationToken ct = default);
    Task AddAsync(Specialization specialization, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}
