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
}
