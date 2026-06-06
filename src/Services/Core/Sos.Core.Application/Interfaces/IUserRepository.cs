using Sos.Core.Domain.Entities.Identity;

namespace Sos.Core.Application.Interfaces;

public interface IUserRepository
{
    Task<List<User>> GetAllAsync(CancellationToken ct = default);
    Task<User?>      GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<User?>      GetByEmailAsync(string email, CancellationToken ct = default);
    Task<bool>       ExistsAsync(string email, CancellationToken ct = default);
    Task             SaveChangesAsync(CancellationToken ct = default);
}
