using Sos.Core.Domain.Entities;

namespace Sos.Core.Application.Interfaces;

public interface IRefreshTokenRepository
{
    Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken ct = default);
    Task                AddAsync(RefreshToken token, CancellationToken ct = default);
    Task                RevokeAsync(string token, CancellationToken ct = default);
}
