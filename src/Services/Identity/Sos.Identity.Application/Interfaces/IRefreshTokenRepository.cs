using Sos.Identity.Domain.Entities;

namespace Sos.Identity.Application.Interfaces;

public interface IRefreshTokenRepository
{
    public Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken ct = default);
    public Task AddAsync(RefreshToken token, CancellationToken ct = default);
    public Task RevokeAsync(string token, CancellationToken ct = default);
}
