using ecom_backend.DTOs.Business;
using ecom_backend.Interfaces.Repositories;
using ecom_backend.Interfaces.Services;
using ecom_backend.Models;

namespace ecom_backend.Services;

public class BusinessProfileService(
    IBusinessProfileRepository businessProfileRepository,
    IFileStorage fileStorage) : IBusinessProfileService
{
    private const long MaxLogoBytes = 5 * 1024 * 1024; // 5 MB

    private static readonly HashSet<string> AllowedImageTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        "image/jpeg",
        "image/png",
        "image/webp",
        "image/gif"
    };

    public async Task<BusinessProfileDto> GetAsync(
        int userId,
        CancellationToken cancellationToken = default)
    {
        var model = await businessProfileRepository.GetByUserIdAsync(userId, cancellationToken);
        return model is null ? new BusinessProfileDto() : ToDto(model);
    }

    public async Task<BusinessProfileDto> SaveAsync(
        int userId,
        SaveBusinessProfileRequest request,
        CancellationToken cancellationToken = default)
    {
        var existing = await businessProfileRepository.GetByUserIdAsync(userId, cancellationToken);

        var model = new BusinessProfileModel
        {
            UserId = userId,
            BusinessName = request.BusinessName?.Trim(),
            Category = request.Category?.Trim(),
            Description = request.Description?.Trim(),
            Website = request.Website?.Trim(),
            // Keep the stored logo when the client doesn't send a new URL.
            LogoUrl = string.IsNullOrWhiteSpace(request.LogoUrl)
                ? existing?.LogoUrl
                : request.LogoUrl.Trim(),
            PrimaryColor = request.PrimaryColor?.Trim(),
            SecondaryColor = request.SecondaryColor?.Trim(),
            Location = request.Location?.Trim()
        };

        var saved = await businessProfileRepository.UpsertAsync(model, cancellationToken);
        return ToDto(saved);
    }

    public async Task<BusinessProfileDto> UploadLogoAsync(
        int userId,
        Stream content,
        string fileName,
        string contentType,
        long length,
        CancellationToken cancellationToken = default)
    {
        if (length <= 0)
            throw new ArgumentException("The uploaded file is empty.");

        if (length > MaxLogoBytes)
            throw new ArgumentException("Image must be 5 MB or smaller.");

        if (string.IsNullOrWhiteSpace(contentType) || !AllowedImageTypes.Contains(contentType))
            throw new ArgumentException("Only JPG, PNG, WEBP, or GIF images are allowed.");

        var existing = await businessProfileRepository.GetByUserIdAsync(userId, cancellationToken);
        var previousUrl = existing?.LogoUrl;

        var url = await fileStorage.SaveAsync(
            content, fileName, contentType, "logos", cancellationToken);

        var model = existing ?? new BusinessProfileModel { UserId = userId };
        model.UserId = userId;
        model.LogoUrl = url;

        var saved = await businessProfileRepository.UpsertAsync(model, cancellationToken);

        if (!string.IsNullOrEmpty(previousUrl))
            await fileStorage.DeleteAsync(previousUrl, cancellationToken);

        return ToDto(saved);
    }

    private static BusinessProfileDto ToDto(BusinessProfileModel model) => new()
    {
        BusinessName = model.BusinessName,
        Category = model.Category,
        Description = model.Description,
        Website = model.Website,
        LogoUrl = model.LogoUrl,
        PrimaryColor = model.PrimaryColor,
        SecondaryColor = model.SecondaryColor,
        Location = model.Location
    };
}
