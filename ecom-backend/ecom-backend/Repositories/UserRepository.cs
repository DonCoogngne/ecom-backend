using ecom_backend.Data;
using ecom_backend.Interfaces.Repositories;
using ecom_backend.Mappings;
using ecom_backend.Models;
using Microsoft.EntityFrameworkCore;

namespace ecom_backend.Repositories;

public class UserRepository(AppDbContext db, ILogger<UserRepository> logger) : IUserRepository
{
    public async Task<bool> EmailExistsAsync(
        string email,
        CancellationToken cancellationToken = default)
    {
        try
        {
            return await db.Users.AnyAsync(u => u.Email == email, cancellationToken);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            logger.LogError(ex, "Failed to check email existence for {Email}", email);
            throw;
        }
    }

    public async Task<UserModel?> GetByEmailAsync( string email,CancellationToken cancellationToken = default)
    {
        try
        {
            var entity = await db.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);

            return entity?.ToModel();
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            logger.LogError(ex, "Failed to get user by email {Email}", email);
            throw;
        }
    }

    public async Task<UserModel?> GetByEmailOrGoogleIdAsync( string email, string googleId, CancellationToken cancellationToken = default)
    {
        try
        {
            var entity = await db.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(
                    u => u.Email == email || u.GoogleId == googleId,
                    cancellationToken);

            return entity?.ToModel();
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            logger.LogError(ex, "Failed to get user by email {Email} or GoogleId", email);
            throw;
        }
    }

    public async Task<UserModel> AddAsync( UserModel user,CancellationToken cancellationToken = default)
    {
        try
        {
            var entity = user.ToEntity();
            db.Users.Add(entity);
            await db.SaveChangesAsync(cancellationToken);
            await db.Entry(entity).Reference(u => u.Role).LoadAsync(cancellationToken);
            return entity.ToModel();
        }
        catch (DbUpdateException ex)
        {
            logger.LogError(ex, "Failed to add user with email {Email}", user.Email);
            throw new InvalidOperationException(
                "Unable to create user. The email may already exist or data is invalid.", ex);
        }
        catch (Exception ex) when (ex is not OperationCanceledException and not InvalidOperationException)
        {
            logger.LogError(ex, "Unexpected error while adding user {Email}", user.Email);
            throw;
        }
    }

    public async Task<UserModel> UpdateAsync( UserModel user, CancellationToken cancellationToken = default)
    {
        try
        {
            var entity = await db.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.UserId == user.UserId, cancellationToken)
                ?? throw new InvalidOperationException("User was not found.");

            user.ApplyToEntity(entity);
            await db.SaveChangesAsync(cancellationToken);
            return entity.ToModel();
        }
        catch (InvalidOperationException)
        {
            throw;
        }
        catch (DbUpdateException ex)
        {
            logger.LogError(ex, "Failed to update user {UserId}", user.UserId);
            throw new InvalidOperationException(
                "Unable to update user. Please try again later.", ex);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            logger.LogError(ex, "Unexpected error while updating user {UserId}", user.UserId);
            throw;
        }
    }
}
