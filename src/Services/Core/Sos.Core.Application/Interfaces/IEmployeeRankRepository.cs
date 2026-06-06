using Sos.Core.Domain.Entities;

namespace Sos.Core.Application.Interfaces;

public interface IEmployeeRankRepository
{
    Task<EmployeeRank?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<EmployeeRank?> GetByCodeAsync(string code, CancellationToken ct = default);
    Task<List<EmployeeRank>> GetAllAsync(CancellationToken ct = default);
    Task<bool> CodeExistsAsync(string code, CancellationToken ct = default);
    Task AddAsync(EmployeeRank rank, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}
