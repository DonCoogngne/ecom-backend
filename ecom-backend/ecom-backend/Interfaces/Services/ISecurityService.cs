using ecom_backend.DTOs.Security;

namespace ecom_backend.Interfaces.Services;

public interface ISecurityService
{
    Task<SecurityDto> GetAsync(int userId, CancellationToken cancellationToken = default);

    Task<SecurityDto> SetTwoFactorAsync(
        int userId,
        bool enabled,
        CancellationToken cancellationToken = default);
}
