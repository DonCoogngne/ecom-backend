namespace ecom_backend.DTOs.Auth;

/// <summary>
/// Internal result returned by the auth service: the API response body plus the
/// raw refresh token the controller writes into an HttpOnly cookie.
/// </summary>
public class AuthResult
{
    public AuthResponse Response { get; set; } = null!;
    public string RefreshToken { get; set; } = string.Empty;
    public DateTime RefreshTokenExpiresAt { get; set; }
}
