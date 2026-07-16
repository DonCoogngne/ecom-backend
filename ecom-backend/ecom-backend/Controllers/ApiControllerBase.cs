using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace ecom_backend.Controllers;

[ApiController]
public abstract class ApiControllerBase : ControllerBase
{
    protected int CurrentUserId
    {
        get
        {
            var value = User.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? User.FindFirstValue("sub");

            if (int.TryParse(value, out var userId))
                return userId;

            throw new UnauthorizedAccessException("User identity could not be resolved.");
        }
    }
}
