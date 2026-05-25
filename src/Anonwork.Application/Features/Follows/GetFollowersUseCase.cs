using Anonwork.Application.Features.Follows.DTOs;
using Anonwork.Application.Interfaces;
using Anonwork.Domain.Entities;

namespace Anonwork.Application.Features.Follows;

/// <summary>
/// Use case for getting followers of a user with pagination
/// </summary>
public class GetFollowersUseCase(IFollowRepository followRepository)
{
    public async Task<PaginatedFollowResponseDto> ExecuteAsync(
        Guid userId,
        int page = 1,
        int pageSize = 10,
        CancellationToken ct = default)
    {
        // ── Validate input ──────────────────────────
        if (userId == Guid.Empty)
            throw new ArgumentException("User ID is required.");

        if (page < 1)
            throw new ArgumentException("Page must be greater than 0.");

        if (pageSize < 1 || pageSize > 100)
            throw new ArgumentException("Page size must be between 1 and 100.");

        // ── Get followers ───────────────────────────
        var (followers, total) = await followRepository.GetFollowersAsync(userId, page, pageSize, ct);

        // ── Map to DTO ──────────────────────────────
        var followDtos = followers.Select(MapToResponse).ToList();

        return new PaginatedFollowResponseDto
        {
            Data = followDtos,
            Page = page,
            PageSize = pageSize,
            Total = total,
            TotalPages = (int)Math.Ceiling(total / (double)pageSize)
        };
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
