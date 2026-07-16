using ecom_backend.DTOs.Business;
using ecom_backend.Interfaces.Repositories;
using ecom_backend.Interfaces.Services;
using ecom_backend.Models;

namespace ecom_backend.Services;

public class BusinessProfileService(
    IBusinessProfileRepository businessProfileRepository) : IBusinessProfileService
{
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
        var model = new BusinessProfileModel
        {
            UserId = userId,
            BusinessName = request.BusinessName?.Trim(),
            Category = request.Category?.Trim(),
            Description = request.Description?.Trim(),
            Website = request.Website?.Trim(),
            LogoUrl = request.LogoUrl?.Trim(),
            PrimaryColor = request.PrimaryColor?.Trim(),
            SecondaryColor = request.SecondaryColor?.Trim(),
            Location = request.Location?.Trim()
        };

        var saved = await businessProfileRepository.UpsertAsync(model, cancellationToken);
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
