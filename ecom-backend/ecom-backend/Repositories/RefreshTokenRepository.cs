using ecom_backend.Data;
using ecom_backend.Interfaces.Repositories;
using ecom_backend.Mappings;
using ecom_backend.Models;
using Microsoft.EntityFrameworkCore;

namespace ecom_backend.Repositories;

public class RefreshTokenRepository(
    AppDbContext db,
    ILogger<RefreshTokenRepository> logger) : IRefreshTokenRepository
{
    public async Task<RefreshTokenModel> AddAsync(
        RefreshTokenModel token,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var entity = token.ToEntity();
            db.RefreshTokens.Add(entity);
            await db.SaveChangesAsync(cancellationToken);
            return entity.ToModel();
        }
        catch (DbUpdateException ex)
        {
            logger.LogError(ex, "Failed to add refresh token for user {UserId}", token.UserId);
            throw new InvalidOperationException("Unable to store refresh token.", ex);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            logger.LogError(ex, "Unexpected error adding refresh token for user {UserId}", token.UserId);
            throw;
        }
    }

    public async Task<RefreshTokenModel?> GetByHashAsync(
        string tokenHash,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var entity = await db.RefreshTokens
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.TokenHash == tokenHash, cancellationToken);

            return entity?.ToModel();
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            logger.LogError(ex, "Failed to look up refresh token");
            throw;
        }
    }

    public async Task RevokeAsync(
        string tokenHash,
        string? revokedByIp,
        string? replacedByTokenHash,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var entity = await db.RefreshTokens
                .FirstOrDefaultAsync(t => t.TokenHash == tokenHash, cancellationToken);

            if (entity is null || entity.RevokedAt is not null)
                return;

            entity.RevokedAt = DateTime.UtcNow;
            entity.RevokedByIp = revokedByIp;
            entity.ReplacedByTokenHash = replacedByTokenHash;
            await db.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            logger.LogError(ex, "Failed to revoke refresh token");
            throw;
        }
    }

    public async Task RevokeAllActiveForUserAsync(
        int userId,
        string? revokedByIp,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var now = DateTime.UtcNow;
            var active = await db.RefreshTokens
                .Where(t => t.UserId == userId && t.RevokedAt == null && t.ExpiresAt > now)
                .ToListAsync(cancellationToken);

            foreach (var entity in active)
            {
                entity.RevokedAt = now;
                entity.RevokedByIp = revokedByIp;
            }

            if (active.Count > 0)
                await db.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            logger.LogError(ex, "Failed to revoke active refresh tokens for user {UserId}", userId);
            throw;
        }
    }
}
