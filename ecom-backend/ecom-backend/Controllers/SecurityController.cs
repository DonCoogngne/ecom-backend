using ecom_backend.DTOs.Security;
using ecom_backend.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ecom_backend.Controllers;

[Authorize]
[Route("api/security")]
public class SecurityController(ISecurityService securityService) : ApiControllerBase
{
    [HttpGet]
    public async Task<ActionResult<SecurityDto>> Get(CancellationToken cancellationToken)
    {
        try
        {
            return Ok(await securityService.GetAsync(CurrentUserId, cancellationToken));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpPut("two-factor")]
    public async Task<ActionResult<SecurityDto>> SetTwoFactor(
        [FromBody] UpdateTwoFactorRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            return Ok(await securityService.SetTwoFactorAsync(CurrentUserId, request.Enabled, cancellationToken));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }
}
