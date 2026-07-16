using ecom_backend.DTOs.Profile;
using ecom_backend.Interfaces.Repositories;
using ecom_backend.Interfaces.Services;
using ecom_backend.Models;

namespace ecom_backend.Services;

public class ProfileService(
    IUserRepository userRepository,
    IFileStorage fileStorage) : IProfileService
{
    private const long MaxAvatarBytes = 5 * 1024 * 1024; // 5 MB

    private static readonly HashSet<string> AllowedImageTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        "image/jpeg",
        "image/png",
        "image/webp",
        "image/gif"
    };

    public async Task<ProfileDto> GetProfileAsync(
        int userId,
        CancellationToken cancellationToken = default)
    {
        var user = await userRepository.GetByIdAsync(userId, cancellationToken)
            ?? throw new KeyNotFoundException("User was not found.");

        return ToDto(user);
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

        return ToDto(user);
    }

    public async Task<ProfileDto> UploadAvatarAsync(
        int userId,
        Stream content,
        string fileName,
        string contentType,
        long length,
        CancellationToken cancellationToken = default)
    {
        if (length <= 0)
            throw new ArgumentException("The uploaded file is empty.");

        if (length > MaxAvatarBytes)
            throw new ArgumentException("Image must be 5 MB or smaller.");

        if (string.IsNullOrWhiteSpace(contentType) || !AllowedImageTypes.Contains(contentType))
            throw new ArgumentException("Only JPG, PNG, WEBP, or GIF images are allowed.");

        var user = await userRepository.GetByIdAsync(userId, cancellationToken)
            ?? throw new KeyNotFoundException("User was not found.");

        var previousUrl = user.ProfileImageUrl;

        var url = await fileStorage.SaveAsync(
            content, fileName, contentType, "avatars", cancellationToken);

        user.ProfileImageUrl = url;
        user = await userRepository.UpdateAsync(user, cancellationToken);

        // Best-effort cleanup of the previously stored avatar.
        if (!string.IsNullOrEmpty(previousUrl))
            await fileStorage.DeleteAsync(previousUrl, cancellationToken);

        return ToDto(user);
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

    private static ProfileDto ToDto(UserModel user) => new()
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
