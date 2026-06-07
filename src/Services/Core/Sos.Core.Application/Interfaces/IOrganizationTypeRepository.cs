using Sos.Core.Domain.Entities;

namespace Sos.Core.Application.Interfaces;

public interface IOrganizationTypeRepository
{
    Task<List<OrganizationType>> GetAllAsync(CancellationToken ct = default);
    Task<OrganizationType?>      GetByIdAsync(Guid id, CancellationToken ct = default);
    Task                         AddAsync(OrganizationType orgType, CancellationToken ct = default);
    Task                         SaveChangesAsync(CancellationToken ct = default);
    Task                         DeleteAsync(OrganizationType orgType, CancellationToken ct = default);
}
