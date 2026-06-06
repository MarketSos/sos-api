using Sos.Core.Domain.Entities;

namespace Sos.Core.Application.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<User?> GetByEmailAsync(string email, CancellationToken ct = default);
    Task<bool>  ExistsAsync(string email, CancellationToken ct = default);
    Task        AddAsync(User user, CancellationToken ct = default);
    Task        SaveChangesAsync(CancellationToken ct = default);
}
