using ecom_backend.DTOs.Subscription;
using ecom_backend.Interfaces.Repositories;
using ecom_backend.Interfaces.Services;

namespace ecom_backend.Services;

public class SubscriptionService(
    ISubscriptionRepository subscriptionRepository) : ISubscriptionService
{
    public async Task<SubscriptionDto> GetAsync(
        int userId,
        CancellationToken cancellationToken = default)
    {
        var model = await subscriptionRepository.GetByUserIdAsync(userId, cancellationToken)
            ?? await subscriptionRepository.CreateDefaultAsync(userId, cancellationToken);

        return new SubscriptionDto
        {
            PlanName = model.PlanName,
            FreeCredits = model.FreeCredits,
            RemainingCredits = model.RemainingCredits,
            Invoices = model.Invoices
                .Select(i => new InvoiceDto
                {
                    InvoiceNumber = i.InvoiceNumber,
                    InvoiceDate = i.InvoiceDate,
                    Amount = i.Amount,
                    Status = i.Status
                })
                .ToList()
        };
    }
}
