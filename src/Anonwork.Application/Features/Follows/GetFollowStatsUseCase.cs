using Anonwork.Application.Features.Follows.DTOs;
using Anonwork.Application.Interfaces;
using Anonwork.Domain.Entities;
using Anonwork.Domain.Enums;

namespace Anonwork.Application.Features.Follows;

/// <summary>
/// Use case for getting follow statistics for a user
/// </summary>
public class GetFollowStatsUseCase(IUnitOfWork unitOfWork)
{
    private readonly IGenericRepository<Follow> _followRepository = unitOfWork.GetRepository<Follow>();
    public async Task<FollowStatsDto> ExecuteAsync(
        Guid userId,
        Guid? currentUserId = null,
        CancellationToken ct = default)
    {
        // ── Validate input ──────────────────────────
        if (userId == Guid.Empty)
            throw new ArgumentException("User ID is required.");

        // ── Get follower count ──────────────────────
        var followerCount = await _followRepository.CountAsync(f => f.Following.Id == userId && 
                                                                f.Following.Status == UserStatus.Active);

        // ── Get following count ─────────────────────
        var followingCount = await _followRepository.CountAsync(f => f.Follower.Id == userId &&
                                                                f.Follower.Status == UserStatus.Active);

        // ── Check if current user is following ──────
        var isFollowing = false;
        if (currentUserId.HasValue && currentUserId.Value != Guid.Empty)
        {
            isFollowing = await _followRepository.ExistsAsync(f => f.FollowerId == currentUserId.Value && f.FollowingId == userId);
        }

        return new FollowStatsDto
        {
            FollowerCount = followerCount,
            FollowingCount = followingCount,
            IsFollowing = isFollowing
        };
    }
}
