using ecom_backend.DTOs.Security;
using ecom_backend.Interfaces.Repositories;
using ecom_backend.Interfaces.Services;

namespace ecom_backend.Services;

public class SecurityService(IUserRepository userRepository) : ISecurityService
{
    public async Task<SecurityDto> GetAsync(
        int userId,
        CancellationToken cancellationToken = default)
    {
        var user = await userRepository.GetByIdAsync(userId, cancellationToken)
            ?? throw new KeyNotFoundException("User was not found.");

        return new SecurityDto { TwoFactorEnabled = user.TwoFactorEnabled };
    }

    public async Task<SecurityDto> SetTwoFactorAsync(
        int userId,
        bool enabled,
        CancellationToken cancellationToken = default)
    {
        var user = await userRepository.GetByIdAsync(userId, cancellationToken)
            ?? throw new KeyNotFoundException("User was not found.");

        user.TwoFactorEnabled = enabled;
        user = await userRepository.UpdateAsync(user, cancellationToken);

        return new SecurityDto { TwoFactorEnabled = user.TwoFactorEnabled };
    }
}
