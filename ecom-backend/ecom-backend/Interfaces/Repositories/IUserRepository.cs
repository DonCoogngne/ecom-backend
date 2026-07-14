using ecom_backend.Models;

namespace ecom_backend.Interfaces.Repositories;

public interface IUserRepository
{
    Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default);

    Task<UserModel?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);

    Task<UserModel?> GetByEmailOrGoogleIdAsync(
        string email,
        string googleId,
        CancellationToken cancellationToken = default);

    Task<UserModel> AddAsync(UserModel user, CancellationToken cancellationToken = default);

    Task<UserModel> UpdateAsync(UserModel user, CancellationToken cancellationToken = default);
}
