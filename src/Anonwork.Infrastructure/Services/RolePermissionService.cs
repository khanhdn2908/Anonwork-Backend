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
    private static string PermissionCacheKey(Guid userId) => $"perm:{userId}";
    private static string RoleCacheKey(Guid userId) => $"role:{userId}";

    public async Task<IReadOnlyCollection<string>> GetPermissionCodesAsync(Guid userId, CancellationToken ct = default)
    {
        var cached = await cache.GetStringAsync(PermissionCacheKey(userId), ct);
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
            PermissionCacheKey(userId),
            JsonSerializer.Serialize(permissions),
            new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10) },
            ct);

        return permissions;
    }

    public async Task<IReadOnlyCollection<string>> GetRoleCodesAsync(Guid userId, CancellationToken ct = default)
    {
        var cached = await cache.GetStringAsync(RoleCacheKey(userId), ct);
        if (!string.IsNullOrWhiteSpace(cached))
        {
            return JsonSerializer.Deserialize<List<string>>(cached) ?? [];
        }

        var roles = await dbContext.Users
            .AsNoTracking()
            .Where(u => u.Id == userId)
            .SelectMany(u => u.UserRoles)
            .Select(ur => ur.Role.Name)
            .Distinct()
            .ToListAsync(ct);

        await cache.SetStringAsync(
            RoleCacheKey(userId),
            JsonSerializer.Serialize(roles),
            new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10) },
            ct);

        return roles;
    }
}
