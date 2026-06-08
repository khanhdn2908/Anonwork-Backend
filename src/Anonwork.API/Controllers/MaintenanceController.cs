using Anonwork.Application.Features.Maintenance;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Anonwork.API.Controllers;

[ApiController]
[Route("api/v1/maintenance")]
public class MaintenanceController(
    CleanupEmailVerificationTokensUseCase cleanupEmailVerificationTokensUseCase,
    CleanupUnpaidExpiredOrdersUseCase cleanupUnpaidExpiredOrdersUseCase,
    IConfiguration configuration) : ControllerBase
{
    [HttpPost("cleanup-email-verification-tokens")]
    public async Task<IActionResult> CleanupEmailVerificationTokens(
        [FromHeader(Name = "X-Maintenance-Secret")] string? secret,
        CancellationToken ct)
    {
        var expectedSecret = configuration["Maintenance:CleanupSecret"];
        if (string.IsNullOrWhiteSpace(expectedSecret) || !string.Equals(secret, expectedSecret, StringComparison.Ordinal))
            return Unauthorized();

        var deletedCount = await cleanupEmailVerificationTokensUseCase.ExecuteAsync(ct);
        return Ok(new { deletedCount });
    }

    [HttpPost("cleanup-unpaid-expired-orders")]
    public async Task<IActionResult> CleanupUnpaidExpiredOrders(
        [FromHeader(Name = "X-Maintenance-Secret")] string? secret,
        CancellationToken ct)
    {
        var expectedSecret = configuration["Maintenance:CleanupSecret"];
        if (string.IsNullOrWhiteSpace(expectedSecret) || !string.Equals(secret, expectedSecret, StringComparison.Ordinal))
            return Unauthorized();

        var deletedCount = await cleanupUnpaidExpiredOrdersUseCase.ExecuteAsync(ct);
        return Ok(new { deletedCount });
    }
}
