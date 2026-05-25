using Anonwork.Application.Interfaces;
using Anonwork.Domain.Entities;
using Anonwork.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Anonwork.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for Follow entity
/// </summary>
public class FollowRepository(AppDbContext context) : IFollowRepository
{
    public async Task<Follow?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await context.Follows
            .AsNoTracking()
            .Include(f => f.Follower)
            .Include(f => f.Following)
            .FirstOrDefaultAsync(f => f.Id == id, ct);
    }

    public async Task<Follow?> GetByFollowerAndFollowingAsync(Guid followerId, Guid followingId, CancellationToken ct = default)
    {
        return await context.Follows
            .AsNoTracking()
            .Include(f => f.Follower)
            .Include(f => f.Following)
            .FirstOrDefaultAsync(f => f.FollowerId == followerId && f.FollowingId == followingId, ct);
    }

    public async Task<Follow> CreateAsync(Follow follow, CancellationToken ct = default)
    {
        context.Follows.Add(follow);
        await context.SaveChangesAsync(ct);
        return follow;
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var follow = await context.Follows.FirstOrDefaultAsync(f => f.Id == id, ct);
        if (follow is not null)
        {
            context.Follows.Remove(follow);
            await context.SaveChangesAsync(ct);
        }
    }

    public async Task DeleteByFollowerAndFollowingAsync(Guid followerId, Guid followingId, CancellationToken ct = default)
    {
        var follow = await context.Follows
            .FirstOrDefaultAsync(f => f.FollowerId == followerId && f.FollowingId == followingId, ct);
        
        if (follow is not null)
        {
            context.Follows.Remove(follow);
            await context.SaveChangesAsync(ct);
        }
    }

    public async Task<bool> ExistsByFollowerAndFollowingAsync(Guid followerId, Guid followingId, CancellationToken ct = default)
    {
        return await context.Follows
            .AnyAsync(f => f.FollowerId == followerId && f.FollowingId == followingId, ct);
    }

    public async Task<(List<Follow> Followers, int Total)> GetFollowersAsync(
        Guid userId,
        int page = 1,
        int pageSize = 10,
        CancellationToken ct = default)
    {
        var query = context.Follows
            .AsNoTracking()
            .Where(f => f.FollowingId == userId)
            .OrderByDescending(f => f.CreatedAt);

        var total = await query.CountAsync(ct);

        var followers = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Include(f => f.Follower)
            .Include(f => f.Following)
            .ToListAsync(ct);

        return (followers, total);
    }

    public async Task<(List<Follow> Following, int Total)> GetFollowingAsync(
        Guid userId,
        int page = 1,
        int pageSize = 10,
        CancellationToken ct = default)
    {
        var query = context.Follows
            .AsNoTracking()
            .Where(f => f.FollowerId == userId)
            .OrderByDescending(f => f.CreatedAt);

        var total = await query.CountAsync(ct);

        var following = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Include(f => f.Follower)
            .Include(f => f.Following)
            .ToListAsync(ct);

        return (following, total);
    }

    public async Task<int> GetFollowerCountAsync(Guid userId, CancellationToken ct = default)
    {
        return await context.Follows
            .CountAsync(f => f.FollowingId == userId, ct);
    }

    public async Task<int> GetFollowingCountAsync(Guid userId, CancellationToken ct = default)
    {
        return await context.Follows
            .CountAsync(f => f.FollowerId == userId, ct);
    }

    public async Task<bool> IsFollowingAsync(Guid followerId, Guid followingId, CancellationToken ct = default)
    {
        return await context.Follows
            .AnyAsync(f => f.FollowerId == followerId && f.FollowingId == followingId, ct);
    }
}
