using ecom_backend.DTOs.Auth;
using ecom_backend.Interfaces.Repositories;
using ecom_backend.Interfaces.Services;
using ecom_backend.Models;
using ecom_backend.Security;
using Google.Apis.Auth;

namespace ecom_backend.Services;

public class AuthService(
    IUserRepository userRepository,
    IRoleRepository roleRepository,
    IRefreshTokenRepository refreshTokenRepository,
    IUserAuthenticationRepository userAuthenticationRepository,
    IJwtTokenService jwtTokenService,
    IConfiguration configuration) : IAuthService
{
    private const string DefaultRoleName = "Customer";
    private const string ProviderEmail = "Email";
    private const string ProviderGoogle = "Google";

    private int RefreshTokenExpiresDays =>
        int.TryParse(configuration["Jwt:RefreshTokenExpiresDays"], out var days) ? days : 7;

    public async Task<AuthResult> SignupAsync(
        SignupRequest request,
        string? ipAddress = null,
        string? deviceName = null,
        CancellationToken cancellationToken = default)
    {
        var email = request.Email.Trim().ToLowerInvariant();

        if (await userRepository.EmailExistsAsync(email, cancellationToken))
            throw new InvalidOperationException("An account with this email already exists.");

        var role = await roleRepository.GetByNameAsync(DefaultRoleName, cancellationToken)
            ?? throw new InvalidOperationException("Default role is not configured.");

        var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

        var user = new UserModel
        {
            FirstName = request.FirstName.Trim(),
            LastName = request.LastName.Trim(),
            Email = email,
            PasswordHash = passwordHash,
            CreatedDate = DateTime.UtcNow,
            IsActive = true,
            RoleId = role.RoleId,
            Role = role
        };

        user = await userRepository.AddAsync(user, cancellationToken);

        await userAuthenticationRepository.UpsertAsync(
            user.UserId, ProviderEmail, null, passwordHash, touchLastLogin: true, cancellationToken);

        return await IssueTokensAsync(user, ipAddress, deviceName, cancellationToken);
    }

    public async Task<AuthResult> LoginAsync(
        LoginRequest request,
        string? ipAddress = null,
        string? deviceName = null,
        CancellationToken cancellationToken = default)
    {
        var email = request.Email.Trim().ToLowerInvariant();
        var user = await userRepository.GetByEmailAsync(email, cancellationToken);

        if (user is null || !user.IsActive)
            throw new UnauthorizedAccessException("Invalid email or password.");

        if (string.IsNullOrEmpty(user.PasswordHash) ||
            !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            throw new UnauthorizedAccessException("Invalid email or password.");

        await userAuthenticationRepository.UpsertAsync(
            user.UserId, ProviderEmail, null, user.PasswordHash, touchLastLogin: true, cancellationToken);

        return await IssueTokensAsync(user, ipAddress, deviceName, cancellationToken);
    }

    public async Task<AuthResult> GoogleLoginAsync(
        GoogleLoginRequest request,
        string? ipAddress = null,
        string? deviceName = null,
        CancellationToken cancellationToken = default)
    {
        var googleClientId = configuration["Google:ClientId"];
        if (string.IsNullOrWhiteSpace(googleClientId))
            throw new InvalidOperationException("Google Sign-In is not configured on the server.");

        var payload = await GoogleJsonWebSignature.ValidateAsync(
            request.IdToken,
            new GoogleJsonWebSignature.ValidationSettings
            {
                Audience = [googleClientId]
            });

        var email = payload.Email.Trim().ToLowerInvariant();
        var user = await userRepository.GetByEmailOrGoogleIdAsync(
            email,
            payload.Subject,
            cancellationToken);

        if (user is null)
        {
            var role = await roleRepository.GetByNameAsync(DefaultRoleName, cancellationToken)
                ?? throw new InvalidOperationException("Default role is not configured.");

            var names = SplitName(payload.Name, payload.GivenName, payload.FamilyName);
            user = new UserModel
            {
                FirstName = names.first,
                LastName = names.last,
                Email = email,
                GoogleId = payload.Subject,
                ProfileImageUrl = payload.Picture,
                CreatedDate = DateTime.UtcNow,
                IsActive = true,
                RoleId = role.RoleId,
                Role = role
            };
            user = await userRepository.AddAsync(user, cancellationToken);
        }
        else
        {
            if (!user.IsActive)
                throw new UnauthorizedAccessException("This account is inactive.");

            user.GoogleId ??= payload.Subject;
            if (!string.IsNullOrEmpty(payload.Picture))
                user.ProfileImageUrl = payload.Picture;

            user = await userRepository.UpdateAsync(user, cancellationToken);
        }

        await userAuthenticationRepository.UpsertAsync(
            user.UserId, ProviderGoogle, payload.Subject, null, touchLastLogin: true, cancellationToken);

        return await IssueTokensAsync(user, ipAddress, deviceName, cancellationToken);
    }

    public async Task<AuthResult> RefreshAsync(
        string refreshToken,
        string? ipAddress = null,
        string? deviceName = null,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(refreshToken))
            throw new UnauthorizedAccessException("Missing refresh token.");

        var hash = TokenHasher.Hash(refreshToken);
        var stored = await refreshTokenRepository.GetByHashAsync(hash, cancellationToken)
            ?? throw new UnauthorizedAccessException("Invalid refresh token.");

        // Reuse detection: a token that was already revoked is being presented again.
        if (stored.RevokedAt is not null)
        {
            await refreshTokenRepository.RevokeAllActiveForUserAsync(
                stored.UserId, ipAddress, cancellationToken);
            throw new UnauthorizedAccessException("Refresh token has been revoked.");
        }

        if (stored.ExpiresAt <= DateTime.UtcNow)
            throw new UnauthorizedAccessException("Refresh token has expired.");

        var user = await userRepository.GetByIdAsync(stored.UserId, cancellationToken);
        if (user is null || !user.IsActive)
            throw new UnauthorizedAccessException("Account is no longer active.");

        // Rotate: issue a new token, then revoke the old one pointing at the replacement.
        var newRawToken = TokenHasher.GenerateToken();
        var newHash = TokenHasher.Hash(newRawToken);
        var expiresAt = DateTime.UtcNow.AddDays(RefreshTokenExpiresDays);

        await refreshTokenRepository.AddAsync(new RefreshTokenModel
        {
            UserId = user.UserId,
            TokenHash = newHash,
            ExpiresAt = expiresAt,
            CreatedAt = DateTime.UtcNow,
            CreatedByIp = ipAddress,
            DeviceName = deviceName ?? stored.DeviceName
        }, cancellationToken);

        await refreshTokenRepository.RevokeAsync(hash, ipAddress, newHash, cancellationToken);

        return new AuthResult
        {
            Response = BuildResponse(user),
            RefreshToken = newRawToken,
            RefreshTokenExpiresAt = expiresAt
        };
    }

    public async Task LogoutAsync(
        string refreshToken,
        string? ipAddress = null,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(refreshToken))
            return;

        var hash = TokenHasher.Hash(refreshToken);
        await refreshTokenRepository.RevokeAsync(hash, ipAddress, null, cancellationToken);
    }

    private async Task<AuthResult> IssueTokensAsync(
        UserModel user,
        string? ipAddress,
        string? deviceName,
        CancellationToken cancellationToken)
    {
        var rawToken = TokenHasher.GenerateToken();
        var expiresAt = DateTime.UtcNow.AddDays(RefreshTokenExpiresDays);

        await refreshTokenRepository.AddAsync(new RefreshTokenModel
        {
            UserId = user.UserId,
            TokenHash = TokenHasher.Hash(rawToken),
            ExpiresAt = expiresAt,
            CreatedAt = DateTime.UtcNow,
            CreatedByIp = ipAddress,
            DeviceName = deviceName
        }, cancellationToken);

        return new AuthResult
        {
            Response = BuildResponse(user),
            RefreshToken = rawToken,
            RefreshTokenExpiresAt = expiresAt
        };
    }

    private AuthResponse BuildResponse(UserModel user) => new()
    {
        Token = jwtTokenService.CreateToken(user),
        User = new UserDto
        {
            UserId = user.UserId,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            ProfileImageUrl = user.ProfileImageUrl,
            RoleName = user.Role.RoleName
        }
    };

    private static (string first, string last) SplitName(string? fullName, string? given, string? family)
    {
        if (!string.IsNullOrWhiteSpace(given) || !string.IsNullOrWhiteSpace(family))
            return (given?.Trim() ?? "User", family?.Trim() ?? "");

        if (string.IsNullOrWhiteSpace(fullName))
            return ("User", "");

        var parts = fullName.Trim().Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
        return parts.Length == 1 ? (parts[0], "") : (parts[0], parts[1]);
    }
}
