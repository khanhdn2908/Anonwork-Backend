using Anonwork.API.DTOs;
using Anonwork.Application.Common.Exceptions;
using Anonwork.Application.Common.Model;
using Anonwork.Application.Features.Auth;
using Anonwork.Application.Features.Auth.DTOs.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Anonwork.API.Controllers;

[ApiController]
[Route("api/v1/auth")]
public class AuthController(
    RegisterUseCase registerUseCase,
    VerifyEmailUseCase verifyEmailUseCase,
    LoginUseCase loginUseCase,
    GoogleLoginUseCase googleLoginUseCase,
    RefreshTokenUseCase refreshTokenUseCase,
    LogoutUseCase logoutUseCase,
    IConfiguration configuration) : BaseApiController
{
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequestDto req, CancellationToken ct)
    {
        await registerUseCase.ExecuteAsync(
            new RegisterRequest(req.Username, req.Email, req.Password, req.AnonAlias), ct);

        return Accepted(new { message = "Verification email has been created." });
    }

    [HttpPost("verify-email")]
    public async Task<IActionResult> VerifyEmail([FromBody] VerifyEmailRequestDto req, CancellationToken ct)
    {
        var result = await verifyEmailUseCase.ExecuteAsync(new VerifyEmailRequest(req.Email, req.Token), ct);
        return Ok(MapToResponse(result));
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto req,CancellationToken ct)
    {
        var result = await loginUseCase.ExecuteAsync(new LoginRequest(req.Email, req.Password), ct);

        return Ok(MapToResponse(result));
    }

    [HttpPost("google")]
    public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginRequestDto req, CancellationToken ct)
    {
        var result = await googleLoginUseCase.ExecuteAsync(
            new GoogleLoginRequest(req.IdToken, req.AnonAlias), ct);

        return Ok(MapToResponse(result));
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshRequestDto req, CancellationToken ct)
    {
        var result = await refreshTokenUseCase.ExecuteAsync(req.RefreshToken, ct);
        return Ok(MapToResponse(result));
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout([FromBody] LogoutRequestDto req, CancellationToken ct)
    {
        var userId = GetUserIdFromToken();
        if (userId is null) return Unauthorized();

        await logoutUseCase.ExecuteAsync(new LogoutRequest(userId.Value, req.RefreshToken, req.AccessToken), ct);

        return NoContent();
    }

    private static AuthResponseDto MapToResponse(AuthResult result) => 
        new(
            result.AccessToken,
            result.RefreshToken,
            result.UserId,
            result.AnonAlias
        );
}