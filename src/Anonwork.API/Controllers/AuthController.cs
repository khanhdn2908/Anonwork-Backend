using Anonwork.API.DTOs;
using Anonwork.Application.Common.Exceptions;
using Anonwork.Application.UseCases.Auth;
using Microsoft.AspNetCore.Mvc;

namespace Anonwork.API.Controllers;

[ApiController]
[Route("api/v1/auth")]
public class AuthController(
    RegisterUseCase registerUseCase,
    LoginUseCase loginUseCase,
    RefreshTokenUseCase refreshTokenUseCase,
    LogoutUseCase logoutUseCase) : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> Register(
        [FromBody] RegisterRequestDto req,
        CancellationToken ct)
    {
        var result = await registerUseCase.ExecuteAsync(
            new RegisterRequest(req.Username, req.Email, req.Password, req.AnonAlias), ct);

        return CreatedAtAction(nameof(Register), MapToResponse(result));
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(
        [FromBody] LoginRequestDto req,
        CancellationToken ct)
    {
        var result = await loginUseCase.ExecuteAsync(
            new LoginRequest(req.Email, req.Password), ct);

        return Ok(MapToResponse(result));
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh(
        [FromBody] RefreshRequestDto req,
        CancellationToken ct)
    {
        var result = await refreshTokenUseCase.ExecuteAsync(req.RefreshToken, ct);
        return Ok(MapToResponse(result));
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout(
        [FromBody] LogoutRequestDto req,
        CancellationToken ct)
    {
        var userId = GetUserIdFromToken();
        if (userId is null) return Unauthorized();

        await logoutUseCase.ExecuteAsync(
            new LogoutRequest(userId.Value, req.RefreshToken, req.AccessToken), ct);

        return NoContent();
    }

    // ── Helpers ─────────────────────────────────────────

    private static AuthResponseDto MapToResponse(AuthResult result) => new(
        result.AccessToken,
        result.RefreshToken,
        result.UserId,
        result.AnonAlias,
        result.Role);

    private Guid? GetUserIdFromToken()
    {
        var sub = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
               ?? User.FindFirst("sub")?.Value;

        return Guid.TryParse(sub, out var id) ? id : null;
    }
}