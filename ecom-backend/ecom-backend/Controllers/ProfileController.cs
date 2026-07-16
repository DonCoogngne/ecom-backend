using ecom_backend.DTOs.Profile;
using ecom_backend.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ecom_backend.Controllers;

[Authorize]
[Route("api/profile")]
public class ProfileController(IProfileService profileService) : ApiControllerBase
{
    [HttpGet]
    public async Task<ActionResult<ProfileDto>> Get(CancellationToken cancellationToken)
    {
        try
        {
            return Ok(await profileService.GetProfileAsync(CurrentUserId, cancellationToken));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpPut]
    public async Task<ActionResult<ProfileDto>> Update(
        [FromBody] UpdateProfileRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            return Ok(await profileService.UpdateProfileAsync(CurrentUserId, request, cancellationToken));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpPost("avatar")]
    [RequestSizeLimit(6 * 1024 * 1024)]
    public async Task<ActionResult<ProfileDto>> UploadAvatar(
        IFormFile file,
        CancellationToken cancellationToken)
    {
        if (file is null || file.Length == 0)
            return BadRequest(new { message = "No file was uploaded." });

        try
        {
            await using var stream = file.OpenReadStream();
            var result = await profileService.UploadAvatarAsync(
                CurrentUserId, stream, file.FileName, file.ContentType, file.Length, cancellationToken);
            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpPut("password")]
    public async Task<IActionResult> ChangePassword(
        [FromBody] ChangePasswordRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            await profileService.ChangePasswordAsync(CurrentUserId, request, cancellationToken);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (UnauthorizedAccessException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
