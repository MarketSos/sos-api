using Sos.Core.Domain.Entities;

namespace Sos.Core.Application.Interfaces;

public interface IOrgTypeRepository
{
    Task<List<OrgType>>  GetAllAsync(CancellationToken ct = default);
    Task<OrgType?>       GetByIdAsync(Guid id, CancellationToken ct = default);
    Task                 AddAsync(OrgType orgType, CancellationToken ct = default);
    Task                 SaveChangesAsync(CancellationToken ct = default);
    Task                 DeleteAsync(OrgType orgType, CancellationToken ct = default);
}
