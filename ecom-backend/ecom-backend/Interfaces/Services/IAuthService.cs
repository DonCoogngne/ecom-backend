using ecom_backend.DTOs.Auth;

namespace ecom_backend.Interfaces.Services;

public interface IAuthService
{
    Task<AuthResult> SignupAsync(
        SignupRequest request,
        string? ipAddress = null,
        string? deviceName = null,
        CancellationToken cancellationToken = default);

    Task<AuthResult> LoginAsync(
        LoginRequest request,
        string? ipAddress = null,
        string? deviceName = null,
        CancellationToken cancellationToken = default);

    Task<AuthResult> GoogleLoginAsync(
        GoogleLoginRequest request,
        string? ipAddress = null,
        string? deviceName = null,
        CancellationToken cancellationToken = default);

    Task<AuthResult> RefreshAsync(
        string refreshToken,
        string? ipAddress = null,
        string? deviceName = null,
        CancellationToken cancellationToken = default);

    Task LogoutAsync(
        string refreshToken,
        string? ipAddress = null,
        CancellationToken cancellationToken = default);
}
