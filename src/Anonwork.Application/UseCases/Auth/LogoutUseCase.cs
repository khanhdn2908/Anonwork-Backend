using Anonwork.Application.Interfaces;
using System.IdentityModel.Tokens.Jwt;

namespace Anonwork.Application.UseCases.Auth;

public record LogoutRequest(Guid UserId, string RefreshToken, string AccessToken);

public class LogoutUseCase(IJwtService jwtService)
{
    public async Task ExecuteAsync(LogoutRequest req, CancellationToken ct = default)
    {
        // Revoke refresh token khỏi Redis
        await jwtService.RevokeRefreshTokenAsync(req.RefreshToken, ct);

        // Blacklist access token cho đến khi hết hạn
        var jti = ExtractJti(req.AccessToken);
        var remaining = ExtractRemaining(req.AccessToken);

        if (jti is not null && remaining > TimeSpan.Zero)
            await jwtService.BlacklistAccessTokenAsync(jti, remaining, ct);
    }

    private static string? ExtractJti(string accessToken)
    {
        try
        {
            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(accessToken);
            return jwt.Id; // claim "jti"
        }
        catch { return null; }
    }

    private static TimeSpan ExtractRemaining(string accessToken)
    {
        try
        {
            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(accessToken);
            var remaining = jwt.ValidTo - DateTime.UtcNow;
            return remaining > TimeSpan.Zero ? remaining : TimeSpan.Zero;
        }
        catch { return TimeSpan.Zero; }
    }
}