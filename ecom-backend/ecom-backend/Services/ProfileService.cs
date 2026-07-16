using ecom_backend.DTOs.Profile;
using ecom_backend.Interfaces.Repositories;
using ecom_backend.Interfaces.Services;

namespace ecom_backend.Services;

public class ProfileService(IUserRepository userRepository) : IProfileService
{
    public async Task<ProfileDto> GetProfileAsync(
        int userId,
        CancellationToken cancellationToken = default)
    {
        var user = await userRepository.GetByIdAsync(userId, cancellationToken)
            ?? throw new KeyNotFoundException("User was not found.");

        return new ProfileDto
        {
            UserId = user.UserId,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            Phone = user.Phone,
            ProfileImageUrl = user.ProfileImageUrl,
            RoleName = user.Role.RoleName,
            GoogleConnected = !string.IsNullOrEmpty(user.GoogleId),
            HasPassword = !string.IsNullOrEmpty(user.PasswordHash)
        };
    }

    public async Task<ProfileDto> UpdateProfileAsync(
        int userId,
        UpdateProfileRequest request,
        CancellationToken cancellationToken = default)
    {
        var user = await userRepository.GetByIdAsync(userId, cancellationToken)
            ?? throw new KeyNotFoundException("User was not found.");

        user.FirstName = request.FirstName.Trim();
        user.LastName = request.LastName.Trim();
        user.Phone = string.IsNullOrWhiteSpace(request.Phone) ? null : request.Phone.Trim();

        user = await userRepository.UpdateAsync(user, cancellationToken);

        return new ProfileDto
        {
            UserId = user.UserId,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            Phone = user.Phone,
            ProfileImageUrl = user.ProfileImageUrl,
            RoleName = user.Role.RoleName,
            GoogleConnected = !string.IsNullOrEmpty(user.GoogleId),
            HasPassword = !string.IsNullOrEmpty(user.PasswordHash)
        };
    }

    public async Task ChangePasswordAsync(
        int userId,
        ChangePasswordRequest request,
        CancellationToken cancellationToken = default)
    {
        var user = await userRepository.GetByIdAsync(userId, cancellationToken)
            ?? throw new KeyNotFoundException("User was not found.");

        if (!string.IsNullOrEmpty(user.PasswordHash))
        {
            if (string.IsNullOrEmpty(request.CurrentPassword) ||
                !BCrypt.Net.BCrypt.Verify(request.CurrentPassword, user.PasswordHash))
                throw new UnauthorizedAccessException("Current password is incorrect.");
        }

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
        await userRepository.UpdateAsync(user, cancellationToken);
    }
}
