using ecom_backend.Data;
using ecom_backend.Domain.Entities;
using ecom_backend.Interfaces.Repositories;
using ecom_backend.Mappings;
using ecom_backend.Models;
using Microsoft.EntityFrameworkCore;

namespace ecom_backend.Repositories;

public class BusinessProfileRepository(
    AppDbContext db,
    ILogger<BusinessProfileRepository> logger) : IBusinessProfileRepository
{
    public async Task<BusinessProfileModel?> GetByUserIdAsync(
        int userId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var entity = await db.BusinessProfiles
                .FirstOrDefaultAsync(b => b.UserId == userId, cancellationToken);

            return entity?.ToModel();
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            logger.LogError(ex, "Failed to get business profile for user {UserId}", userId);
            throw;
        }
    }

    public async Task<BusinessProfileModel> UpsertAsync(
        BusinessProfileModel model,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var entity = await db.BusinessProfiles
                .FirstOrDefaultAsync(b => b.UserId == model.UserId, cancellationToken);

            if (entity is null)
            {
                entity = new BusinessProfile
                {
                    UserId = model.UserId,
                    CreatedDate = DateTime.UtcNow,
                };
                db.BusinessProfiles.Add(entity);
            }

            entity.BusinessName = model.BusinessName;
            entity.Category = model.Category;
            entity.Description = model.Description;
            entity.Website = model.Website;
            entity.LogoUrl = model.LogoUrl;
            entity.PrimaryColor = model.PrimaryColor;
            entity.SecondaryColor = model.SecondaryColor;
            entity.Location = model.Location;
            entity.UpdatedDate = DateTime.UtcNow;

            await db.SaveChangesAsync(cancellationToken);
            return entity.ToModel();
        }
        catch (DbUpdateException ex)
        {
            logger.LogError(ex, "Failed to save business profile for user {UserId}", model.UserId);
            throw new InvalidOperationException(
                "Unable to save business profile. Please try again later.", ex);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            logger.LogError(ex, "Unexpected error saving business profile for user {UserId}", model.UserId);
            throw;
        }
    }
}
