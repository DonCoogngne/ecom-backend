using ecom_backend.DTOs.Auth;
using ecom_backend.Interfaces.Repositories;
using ecom_backend.Interfaces.Services;
using ecom_backend.Models;
using Google.Apis.Auth;

namespace ecom_backend.Services;

public class AuthService(
    IUserRepository userRepository,
    IRoleRepository roleRepository,
    IJwtTokenService jwtTokenService,
    IConfiguration configuration) : IAuthService
{
    private const string DefaultRoleName = "Customer";

    public async Task<AuthResponse> SignupAsync(
        SignupRequest request,
        CancellationToken cancellationToken = default)
    {
        var email = request.Email.Trim().ToLowerInvariant();

        if (await userRepository.EmailExistsAsync(email, cancellationToken))
            throw new InvalidOperationException("An account with this email already exists.");

        var role = await roleRepository.GetByNameAsync(DefaultRoleName, cancellationToken)
            ?? throw new InvalidOperationException("Default role is not configured.");

        var user = new UserModel
        {
            FirstName = request.FirstName.Trim(),
            LastName = request.LastName.Trim(),
            Email = email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            CreatedDate = DateTime.UtcNow,
            IsActive = true,
            RoleId = role.RoleId,
            Role = role
        };

        user = await userRepository.AddAsync(user, cancellationToken);
        return BuildResponse(user);
    }

    public async Task<AuthResponse> LoginAsync(
        LoginRequest request,
        CancellationToken cancellationToken = default)
    {
        var email = request.Email.Trim().ToLowerInvariant();
        var user = await userRepository.GetByEmailAsync(email, cancellationToken);

        if (user is null || !user.IsActive)
            throw new UnauthorizedAccessException("Invalid email or password.");

        if (string.IsNullOrEmpty(user.PasswordHash) ||
            !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            throw new UnauthorizedAccessException("Invalid email or password.");

        return BuildResponse(user);
    }

    public async Task<AuthResponse> GoogleLoginAsync(
        GoogleLoginRequest request,
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

        return BuildResponse(user);
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
