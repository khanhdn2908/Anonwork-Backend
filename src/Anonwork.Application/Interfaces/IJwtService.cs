// Application/Interfaces/IJwtService.cs

using Anonwork.Domain.Entities;

namespace Anonwork.Application.Interfaces;

public interface IJwtService
{
    string GenerateAccessToken(User user, IEnumerable<string> permissions);

    Task<string> GenerateRefreshTokenAsync(Guid userId, CancellationToken ct = default);
    Task<Guid?> ValidateRefreshTokenAsync(string token, CancellationToken ct = default);
    Task RevokeRefreshTokenAsync(string token, CancellationToken ct = default);

    Task BlacklistAccessTokenAsync(string jti, TimeSpan remaining, CancellationToken ct = default);
    Task<bool> IsAccessTokenBlacklistedAsync(string jti, CancellationToken ct = default);
}