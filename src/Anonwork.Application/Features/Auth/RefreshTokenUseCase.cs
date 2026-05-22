using Anonwork.Application.Common.Exceptions;
using Anonwork.Application.Common.Model;
using Anonwork.Application.Interfaces;

namespace Anonwork.Application.Features.Auth;

public class RefreshTokenUseCase(IUserRepository userRepo, IJwtService jwtService)
{
    public async Task<AuthResult> ExecuteAsync(string refreshToken, CancellationToken ct = default)
    {
        // Validate refresh token từ Redis
        var userId = await jwtService.ValidateRefreshTokenAsync(refreshToken, ct)
            ?? throw new UnauthorizedException("Invalid or expired refresh token.");

        var user = await userRepo.GetByIdAsync(userId, ct)
            ?? throw new UnauthorizedException("User not found.");

        // Revoke token cũ, issue cặp mới (rotation)
        await jwtService.RevokeRefreshTokenAsync(refreshToken, ct);

        var newAccessToken = jwtService.GenerateAccessToken(user);
        var newRefreshToken = await jwtService.GenerateRefreshTokenAsync(user.Id, ct);

        return new AuthResult(newAccessToken, newRefreshToken, user.Id, user.AnonAlias, user.Role);
    }
}