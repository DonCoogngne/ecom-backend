using ecom_backend.DTOs.Business;
using ecom_backend.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ecom_backend.Controllers;

[Authorize]
[Route("api/business-profile")]
public class BusinessProfileController(
    IBusinessProfileService businessProfileService) : ApiControllerBase
{
    [HttpGet]
    public async Task<ActionResult<BusinessProfileDto>> Get(CancellationToken cancellationToken)
    {
        return Ok(await businessProfileService.GetAsync(CurrentUserId, cancellationToken));
    }

    [HttpPut]
    public async Task<ActionResult<BusinessProfileDto>> Save(
        [FromBody] SaveBusinessProfileRequest request,
        CancellationToken cancellationToken)
    {
        return Ok(await businessProfileService.SaveAsync(CurrentUserId, request, cancellationToken));
    }

    [HttpPost("logo")]
    [RequestSizeLimit(6 * 1024 * 1024)]
    public async Task<ActionResult<BusinessProfileDto>> UploadLogo(
        IFormFile file,
        CancellationToken cancellationToken)
    {
        if (file is null || file.Length == 0)
            return BadRequest(new { message = "No file was uploaded." });

        try
        {
            await using var stream = file.OpenReadStream();
            var result = await businessProfileService.UploadLogoAsync(
                CurrentUserId, stream, file.FileName, file.ContentType, file.Length, cancellationToken);
            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
