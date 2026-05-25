using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Anonwork.API.Controllers;

[ApiController]
public abstract class BaseApiController : ControllerBase
{
    /// <summary>
    /// Extracts the user ID from the JWT token claims
    /// </summary>
    /// <returns>User ID as Guid, or null if not found or invalid</returns>
    protected Guid? GetUserIdFromToken()
    {
        var sub = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
               ?? User.FindFirst("sub")?.Value;

        return Guid.TryParse(sub, out var id) ? id : null;
    }
}
