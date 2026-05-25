using Anonwork.Application.Features.Follows.DTOs;
using Anonwork.Application.Interfaces;

namespace Anonwork.Application.Features.Follows;

/// <summary>
/// Use case for getting follow statistics for a user
/// </summary>
public class GetFollowStatsUseCase(IFollowRepository followRepository)
{
    public async Task<FollowStatsDto> ExecuteAsync(
        Guid userId,
        Guid? currentUserId = null,
        CancellationToken ct = default)
    {
        // ── Validate input ──────────────────────────
        if (userId == Guid.Empty)
            throw new ArgumentException("User ID is required.");

        // ── Get follower count ──────────────────────
        var followerCount = await followRepository.GetFollowerCountAsync(userId, ct);

        // ── Get following count ─────────────────────
        var followingCount = await followRepository.GetFollowingCountAsync(userId, ct);

        // ── Check if current user is following ──────
        var isFollowing = false;
        if (currentUserId.HasValue && currentUserId.Value != Guid.Empty)
        {
            isFollowing = await followRepository.IsFollowingAsync(currentUserId.Value, userId, ct);
        }

        return new FollowStatsDto
        {
            FollowerCount = followerCount,
            FollowingCount = followingCount,
            IsFollowing = isFollowing
        };
    }
}
