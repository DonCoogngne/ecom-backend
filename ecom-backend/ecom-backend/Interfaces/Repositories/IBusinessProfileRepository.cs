using ecom_backend.Models;

namespace ecom_backend.Interfaces.Repositories;

public interface IBusinessProfileRepository
{
    Task<BusinessProfileModel?> GetByUserIdAsync(
        int userId,
        CancellationToken cancellationToken = default);

    Task<BusinessProfileModel> UpsertAsync(
        BusinessProfileModel model,
        CancellationToken cancellationToken = default);
}
