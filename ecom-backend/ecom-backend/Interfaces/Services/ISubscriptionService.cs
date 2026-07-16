using ecom_backend.DTOs.Subscription;

namespace ecom_backend.Interfaces.Services;

public interface ISubscriptionService
{
    Task<SubscriptionDto> GetAsync(int userId, CancellationToken cancellationToken = default);
}
