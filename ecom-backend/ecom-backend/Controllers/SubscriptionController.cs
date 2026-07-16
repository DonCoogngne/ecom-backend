using ecom_backend.DTOs.Subscription;
using ecom_backend.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ecom_backend.Controllers;

[Authorize]
[Route("api/subscription")]
public class SubscriptionController(
    ISubscriptionService subscriptionService) : ApiControllerBase
{
    [HttpGet]
    public async Task<ActionResult<SubscriptionDto>> Get(CancellationToken cancellationToken)
    {
        return Ok(await subscriptionService.GetAsync(CurrentUserId, cancellationToken));
    }
}
