using Anonwork.Application.Features.Follows.DTOs;
using Anonwork.Application.Interfaces;
using Anonwork.Domain.Entities;

namespace Anonwork.Application.Features.Follows;

/// <summary>
/// Use case for getting a follow relationship by ID
/// </summary>
public class GetFollowByIdUseCase(IFollowRepository followRepository)
{
    public async Task<FollowResponseDto> ExecuteAsync(Guid followId, CancellationToken ct = default)
    {
        // ── Validate input ──────────────────────────
        if (followId == Guid.Empty)
            throw new ArgumentException("Follow ID is required.");

        // ── Get follow relationship ─────────────────
        var follow = await followRepository.GetByIdAsync(followId, ct);
        if (follow == null)
            throw new KeyNotFoundException("Follow relationship not found.");

        // ── Map to DTO ──────────────────────────────
        return MapToResponse(follow);
    }

    private static FollowResponseDto MapToResponse(Follow follow)
    {
        return new FollowResponseDto
        {
            Id = follow.Id,
            FollowerId = follow.FollowerId,
            FollowingId = follow.FollowingId,
            CreatedAt = follow.CreatedAt,
            Follower = follow.Follower != null ? new UserBasicDto
            {
                Id = follow.Follower.Id,
                Username = follow.Follower.Username,
                Email = follow.Follower.Email,
                AvatarUrl = follow.Follower.AvatarUrl
            } : null,
            Following = follow.Following != null ? new UserBasicDto
            {
                Id = follow.Following.Id,
                Username = follow.Following.Username,
                Email = follow.Following.Email,
                AvatarUrl = follow.Following.AvatarUrl
            } : null
        };
    }
}
