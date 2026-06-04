using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Anonwork.Application.Interfaces;
using Anonwork.Domain.Entities;
using Anonwork.Infrastructure.Services;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Anonwork.Infrastructure.Services;

public class JwtService(
    IOptions<JwtOptions> opts,
    IDistributedCache cache) : IJwtService
{
    private readonly JwtOptions _opts = opts.Value;

    // ──────────────────────────────────────────
    // ACCESS TOKEN
    // ──────────────────────────────────────────

    public string GenerateAccessToken(User user, IEnumerable<string> permissions)
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub,   user.Id.ToString()),
            new(JwtRegisteredClaimNames.Jti,   Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email),
            new("username",  user.Username),
            new("anonAlias", user.AnonAlias)
        };

        claims.AddRange(permissions.Select(permission => new Claim("permission", permission)));

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_opts.Secret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _opts.Issuer,
            audience: _opts.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_opts.AccessTokenMinutes),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    // ──────────────────────────────────────────
    // REFRESH TOKEN
    // ──────────────────────────────────────────

    public async Task<string> GenerateRefreshTokenAsync(Guid userId, CancellationToken ct = default)
    {
        await RevokeByUserIdAsync(userId, ct);

        var token = GenerateSecureToken();

        var cacheOpts = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(_opts.RefreshTokenDays)
        };

        await cache.SetStringAsync(RefreshKey(token), userId.ToString(), cacheOpts, ct);
        await cache.SetStringAsync(UserRefreshKey(userId), token, cacheOpts, ct);

        return token;
    }

    public async Task<Guid?> ValidateRefreshTokenAsync(string token, CancellationToken ct = default)
    {
        var value = await cache.GetStringAsync(RefreshKey(token), ct);
        return value is not null && Guid.TryParse(value, out var id) ? id : null;
    }

    public async Task RevokeRefreshTokenAsync(string token, CancellationToken ct = default)
        => await cache.RemoveAsync(RefreshKey(token), ct);

    // ──────────────────────────────────────────
    // BLACKLIST (logout)
    // ──────────────────────────────────────────

    public async Task BlacklistAccessTokenAsync(string jti, TimeSpan remaining, CancellationToken ct = default)
    {
        var cacheOpts = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = remaining
        };
        await cache.SetStringAsync(BlacklistKey(jti), "1", cacheOpts, ct);
    }

    public async Task<bool> IsAccessTokenBlacklistedAsync(string jti, CancellationToken ct = default)
        => await cache.GetStringAsync(BlacklistKey(jti), ct) is not null;

    // ──────────────────────────────────────────
    // PRIVATE HELPERS
    // ──────────────────────────────────────────

    private async Task RevokeByUserIdAsync(Guid userId, CancellationToken ct)
    {
        var oldToken = await cache.GetStringAsync(UserRefreshKey(userId), ct);
        if (oldToken is not null)
        {
            await cache.RemoveAsync(RefreshKey(oldToken), ct);
            await cache.RemoveAsync(UserRefreshKey(userId), ct);
        }
    }

    private static string GenerateSecureToken()
        => Convert.ToBase64String(RandomNumberGenerator.GetBytes(64))
            .Replace("+", "-")
            .Replace("/", "_")
            .TrimEnd('=');

    private static string RefreshKey(string token) => $"rt:{token}";
    private static string UserRefreshKey(Guid userId) => $"rtu:{userId}";
    private static string BlacklistKey(string jti) => $"bl:{jti}";
}