using ecom_backend.Models;

namespace ecom_backend.Interfaces.Repositories;

public interface IRefreshTokenRepository
{
    Task<RefreshTokenModel> AddAsync(
        RefreshTokenModel token,
        CancellationToken cancellationToken = default);

    Task<RefreshTokenModel?> GetByHashAsync(
        string tokenHash,
        CancellationToken cancellationToken = default);

    Task RevokeAsync(
        string tokenHash,
        string? revokedByIp,
        string? replacedByTokenHash,
        CancellationToken cancellationToken = default);

    Task RevokeAllActiveForUserAsync(
        int userId,
        string? revokedByIp,
        CancellationToken cancellationToken = default);
}
