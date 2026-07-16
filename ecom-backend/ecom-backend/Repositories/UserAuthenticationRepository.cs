using ecom_backend.Data;
using ecom_backend.Domain.Entities;
using ecom_backend.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ecom_backend.Repositories;

public class UserAuthenticationRepository(
    AppDbContext db,
    ILogger<UserAuthenticationRepository> logger) : IUserAuthenticationRepository
{
    public async Task UpsertAsync(
        int userId,
        string provider,
        string? providerUserId,
        string? passwordHash,
        bool touchLastLogin,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var entity = await db.UserAuthentications
                .FirstOrDefaultAsync(
                    a => a.UserId == userId && a.Provider == provider,
                    cancellationToken);

            if (entity is null)
            {
                entity = new UserAuthentication
                {
                    UserId = userId,
                    Provider = provider,
                    CreatedDate = DateTime.UtcNow,
                };
                db.UserAuthentications.Add(entity);
            }

            if (providerUserId is not null)
                entity.ProviderUserId = providerUserId;

            if (passwordHash is not null)
                entity.PasswordHash = passwordHash;

            if (touchLastLogin)
                entity.LastLoginDate = DateTime.UtcNow;

            await db.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            // The provider registry is auxiliary; log but don't fail the login flow.
            logger.LogError(ex,
                "Failed to upsert authentication provider {Provider} for user {UserId}",
                provider, userId);
        }
    }
}
