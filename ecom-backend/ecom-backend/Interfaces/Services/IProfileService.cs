using ecom_backend.DTOs.Profile;

namespace ecom_backend.Interfaces.Services;

public interface IProfileService
{
    Task<ProfileDto> GetProfileAsync(int userId, CancellationToken cancellationToken = default);

    Task<ProfileDto> UpdateProfileAsync(
        int userId,
        UpdateProfileRequest request,
        CancellationToken cancellationToken = default);

    Task<ProfileDto> UploadAvatarAsync(
        int userId,
        Stream content,
        string fileName,
        string contentType,
        long length,
        CancellationToken cancellationToken = default);

    Task ChangePasswordAsync(
        int userId,
        ChangePasswordRequest request,
        CancellationToken cancellationToken = default);
}
