using ecom_backend.Data;
using ecom_backend.Domain.Entities;
using ecom_backend.Interfaces.Repositories;
using ecom_backend.Mappings;
using ecom_backend.Models;
using Microsoft.EntityFrameworkCore;

namespace ecom_backend.Repositories;

public class SubscriptionRepository(
    AppDbContext db,
    ILogger<SubscriptionRepository> logger) : ISubscriptionRepository
{
    public async Task<SubscriptionModel?> GetByUserIdAsync(
        int userId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var entity = await db.Subscriptions
                .Include(s => s.Invoices)
                .FirstOrDefaultAsync(s => s.UserId == userId, cancellationToken);

            return entity?.ToModel();
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            logger.LogError(ex, "Failed to get subscription for user {UserId}", userId);
            throw;
        }
    }

    public async Task<SubscriptionModel> CreateDefaultAsync(
        int userId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var entity = new Subscription
            {
                UserId = userId,
                PlanName = "Free",
                FreeCredits = 25,
                RemainingCredits = 25,
                CreatedDate = DateTime.UtcNow,
                UpdatedDate = DateTime.UtcNow,
            };

            db.Subscriptions.Add(entity);
            await db.SaveChangesAsync(cancellationToken);
            return entity.ToModel();
        }
        catch (DbUpdateException ex)
        {
            logger.LogError(ex, "Failed to create default subscription for user {UserId}", userId);
            throw new InvalidOperationException(
                "Unable to create subscription. Please try again later.", ex);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            logger.LogError(ex, "Unexpected error creating subscription for user {UserId}", userId);
            throw;
        }
    }
}
