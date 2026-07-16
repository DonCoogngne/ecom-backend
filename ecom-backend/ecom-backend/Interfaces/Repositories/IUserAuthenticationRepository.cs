namespace ecom_backend.Interfaces.Repositories;

public interface IUserAuthenticationRepository
{
    /// <summary>
    /// Creates or updates the provider registry row for a user, optionally
    /// stamping the last login time.
    /// </summary>
    Task UpsertAsync(
        int userId,
        string provider,
        string? providerUserId,
        string? passwordHash,
        bool touchLastLogin,
        CancellationToken cancellationToken = default);
}
