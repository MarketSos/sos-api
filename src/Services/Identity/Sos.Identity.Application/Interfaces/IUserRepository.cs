using Sos.Identity.Domain.Entities;

namespace Sos.Identity.Application.Interfaces;

public interface IUserRepository
{
    public Task<User?> GetByIdAsync(Guid id, CancellationToken ct = default);
    public Task<User?> GetByEmailAsync(string email, CancellationToken ct = default);
    public Task<bool> ExistsAsync(string email, CancellationToken ct = default);
    public Task AddAsync(User user, CancellationToken ct = default);
    public Task UpdateAsync(User user, CancellationToken ct = default);
}
