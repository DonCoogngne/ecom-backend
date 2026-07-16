using ecom_backend.DTOs.Business;

namespace ecom_backend.Interfaces.Services;

public interface IBusinessProfileService
{
    Task<BusinessProfileDto> GetAsync(int userId, CancellationToken cancellationToken = default);

    Task<BusinessProfileDto> SaveAsync(
        int userId,
        SaveBusinessProfileRequest request,
        CancellationToken cancellationToken = default);
}
