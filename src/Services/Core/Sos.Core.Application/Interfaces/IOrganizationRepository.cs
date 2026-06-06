using Sos.Core.Domain.Entities;

namespace Sos.Core.Application.Interfaces;

public interface IOrganizationRepository
{
    Task<Organization?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<Organization?> GetBySlugAsync(string slug, CancellationToken ct = default);
    Task<Organization?> GetByOwnerAsync(Guid ownerUserId, CancellationToken ct = default);
    Task<Organization?> GetByCodeAsync(string code, CancellationToken ct = default);
    Task<List<Organization>> GetAllAsync(CancellationToken ct = default);
    Task<List<Organization>> GetByParentAsync(Guid parentId, CancellationToken ct = default);
    Task<List<Organization>> GetChildsAsync(Guid parentId, CancellationToken ct = default);
    Task<bool>  SlugExistsAsync(string slug, CancellationToken ct = default);
    Task<bool>  CodeExistsAsync(string code, CancellationToken ct = default);
    Task        AddAsync(Organization organization, CancellationToken ct = default);
    Task        SaveChangesAsync(CancellationToken ct = default);
}
