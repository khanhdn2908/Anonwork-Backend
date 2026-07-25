using System.Security.Claims;
using Anonwork.Application.Features.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Anonwork.API.Controllers;

[ApiController]
[Route("api/v1/admin/activity-logs")]
[Authorize]
public class ActivityLogsController(GetActivityLogsUseCase getActivityLogsUseCase) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(ActivityLogListResponseDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetLogs(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] Guid? userId = null,
        [FromQuery] string? category = null,
        [FromQuery] string? search = null,
        CancellationToken ct = default)
    {
        var result = await getActivityLogsUseCase.ExecuteAsync(page, pageSize, userId, category, search, ct);
        return Ok(result);
    }
}
