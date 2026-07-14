using ecom_backend.Data;
using ecom_backend.Interfaces.Repositories;
using ecom_backend.Mappings;
using ecom_backend.Models;
using Microsoft.EntityFrameworkCore;

namespace ecom_backend.Repositories;

public class UserRepository(AppDbContext db) : IUserRepository
{
    public Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default)
    {
        return db.Users.AnyAsync(u => u.Email == email, cancellationToken);
    }

    public async Task<UserModel?> GetByEmailAsync(
        string email,
        CancellationToken cancellationToken = default)
    {
        var entity = await db.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);

        return entity?.ToModel();
    }

    public async Task<UserModel?> GetByEmailOrGoogleIdAsync(
        string email,
        string googleId,
        CancellationToken cancellationToken = default)
    {
        var entity = await db.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(
                u => u.Email == email || u.GoogleId == googleId,
                cancellationToken);

        return entity?.ToModel();
    }

    public async Task<UserModel> AddAsync(
        UserModel user,
        CancellationToken cancellationToken = default)
    {
        var entity = user.ToEntity();
        db.Users.Add(entity);
        await db.SaveChangesAsync(cancellationToken);
        await db.Entry(entity).Reference(u => u.Role).LoadAsync(cancellationToken);
        return entity.ToModel();
    }

    public async Task<UserModel> UpdateAsync(
        UserModel user,
        CancellationToken cancellationToken = default)
    {
        var entity = await db.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.UserId == user.UserId, cancellationToken)
            ?? throw new InvalidOperationException("User was not found.");

        user.ApplyToEntity(entity);
        await db.SaveChangesAsync(cancellationToken);
        return entity.ToModel();
    }
}
