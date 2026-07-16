using ecom_backend.DTOs.Auth;
using ecom_backend.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace ecom_backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IAuthService authService) : ControllerBase
{
    private const string RefreshCookieName = "refresh_token";
    private const string RefreshCookiePath = "/api/auth";

    [HttpPost("signup")]
    public async Task<ActionResult<AuthResponse>> Signup(
        [FromBody] SignupRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await authService.SignupAsync(
                request, GetIpAddress(), GetDeviceName(), cancellationToken);
            SetRefreshCookie(result);
            return Ok(result.Response);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login(
        [FromBody] LoginRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await authService.LoginAsync(
                request, GetIpAddress(), GetDeviceName(), cancellationToken);
            SetRefreshCookie(result);
            return Ok(result.Response);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
    }

    [HttpPost("google")]
    public async Task<ActionResult<AuthResponse>> Google(
        [FromBody] GoogleLoginRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await authService.GoogleLoginAsync(
                request, GetIpAddress(), GetDeviceName(), cancellationToken);
            SetRefreshCookie(result);
            return Ok(result.Response);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception)
        {
            return BadRequest(new { message = "Google Sign-In failed. Please try again." });
        }
    }

    [HttpPost("refresh")]
    public async Task<ActionResult<AuthResponse>> Refresh(CancellationToken cancellationToken)
    {
        var token = Request.Cookies[RefreshCookieName];
        if (string.IsNullOrEmpty(token))
            return Unauthorized(new { message = "Missing refresh token." });

        try
        {
            var result = await authService.RefreshAsync(
                token, GetIpAddress(), GetDeviceName(), cancellationToken);
            SetRefreshCookie(result);
            return Ok(result.Response);
        }
        catch (UnauthorizedAccessException ex)
        {
            ClearRefreshCookie();
            return Unauthorized(new { message = ex.Message });
        }
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout(CancellationToken cancellationToken)
    {
        var token = Request.Cookies[RefreshCookieName];
        if (!string.IsNullOrEmpty(token))
            await authService.LogoutAsync(token, GetIpAddress(), cancellationToken);

        ClearRefreshCookie();
        return NoContent();
    }

    private void SetRefreshCookie(AuthResult result)
    {
        Response.Cookies.Append(RefreshCookieName, result.RefreshToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.None,
            Expires = result.RefreshTokenExpiresAt,
            Path = RefreshCookiePath,
            IsEssential = true
        });
    }

    private void ClearRefreshCookie()
    {
        Response.Cookies.Append(RefreshCookieName, string.Empty, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.None,
            Expires = DateTimeOffset.UnixEpoch,
            Path = RefreshCookiePath,
            IsEssential = true
        });
    }

    private string? GetIpAddress()
    {
        if (Request.Headers.TryGetValue("X-Forwarded-For", out var forwarded))
            return forwarded.ToString().Split(',')[0].Trim();

        return HttpContext.Connection.RemoteIpAddress?.ToString();
    }

    private string? GetDeviceName()
    {
        var userAgent = Request.Headers.UserAgent.ToString();
        return string.IsNullOrWhiteSpace(userAgent)
            ? null
            : userAgent.Length > 200 ? userAgent[..200] : userAgent;
    }
}
