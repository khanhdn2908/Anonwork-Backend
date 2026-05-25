using Anonwork.API.DTOs;
using Anonwork.Application.Common.Exceptions;
using Anonwork.Application.Common.Model;
using Anonwork.Application.Features.Auth;
using Anonwork.Application.Features.Auth.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Anonwork.API.Controllers;

[ApiController]
[Route("api/v1/auth")]
[Authorize]
public class AuthController(
    RegisterUseCase registerUseCase,
    LoginUseCase loginUseCase,
    RefreshTokenUseCase refreshTokenUseCase,
    LogoutUseCase logoutUseCase) : BaseApiController
{
    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] RegisterRequestDto req, CancellationToken ct)
    {
        var result = await registerUseCase.ExecuteAsync(
            new RegisterRequest(req.Username, req.Email, req.Password, req.AnonAlias), ct);

        return CreatedAtAction(nameof(Register), MapToResponse(result));
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto req,CancellationToken ct)
    {
        var result = await loginUseCase.ExecuteAsync(new LoginRequest(req.Email, req.Password), ct);

        return Ok(MapToResponse(result));
    }

    [HttpPost("refresh")]
    [AllowAnonymous]
    public async Task<IActionResult> Refresh([FromBody] RefreshRequestDto req, CancellationToken ct)
    {
        var result = await refreshTokenUseCase.ExecuteAsync(req.RefreshToken, ct);
        return Ok(MapToResponse(result));
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout([FromBody] LogoutRequestDto req, CancellationToken ct)
    {
        var userId = GetUserIdFromToken();
        if (userId is null) return Unauthorized();

        await logoutUseCase.ExecuteAsync(new LogoutRequest(userId.Value, req.RefreshToken, req.AccessToken), ct);

        return NoContent();
    }

    // ── Helpers ─────────────────────────────────────────

    private static AuthResponseDto MapToResponse(AuthResult result) => 
        new(
            result.AccessToken,
            result.RefreshToken,
            result.UserId,
            result.AnonAlias,
            result.Role
        );
}