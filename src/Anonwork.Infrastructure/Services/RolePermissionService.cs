using Anonwork.Application.Interfaces;
using Anonwork.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace Anonwork.Infrastructure.Services;

public class RolePermissionService(
    IAppDbContext dbContext,
    IDistributedCache cache) : IRolePermissionService
{
    private static string CacheKey(Guid userId) => $"perm:{userId}";

    public async Task<IReadOnlyCollection<string>> GetPermissionCodesAsync(Guid userId, CancellationToken ct = default)
    {
        var cached = await cache.GetStringAsync(CacheKey(userId), ct);
        if (!string.IsNullOrWhiteSpace(cached))
        {
            return JsonSerializer.Deserialize<List<string>>(cached) ?? [];
        }

        var permissions = await dbContext.Users
            .AsNoTracking()
            .Where(u => u.Id == userId)
            .SelectMany(u => u.UserRoles)
            .SelectMany(ur => ur.Role.RolePermissions)
            .Select(rp => rp.Permission.Code)
            .Distinct()
            .ToListAsync(ct);

        await cache.SetStringAsync(
            CacheKey(userId),
            JsonSerializer.Serialize(permissions),
            new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10) },
            ct);

        return permissions;
    }
}
