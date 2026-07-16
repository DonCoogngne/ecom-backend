using ecom_backend.Models;

namespace ecom_backend.Interfaces.Repositories;

public interface ISubscriptionRepository
{
    Task<SubscriptionModel?> GetByUserIdAsync(
        int userId,
        CancellationToken cancellationToken = default);

    Task<SubscriptionModel> CreateDefaultAsync(
        int userId,
        CancellationToken cancellationToken = default);
}
