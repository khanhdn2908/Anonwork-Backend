using Anonwork.Application.Features.Follows.DTOs;
using Anonwork.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using Anonwork.Domain.Entities;

namespace Anonwork.Application.Features.Follows;

/// <summary>
/// Use case for getting users that a user is following with pagination
/// </summary>
public class GetFollowingUseCase(IUnitOfWork unitOfWork)
{
    private readonly IGenericRepository<Follow> _followRepository = unitOfWork.GetRepository<Follow>();

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

        // ── Get following ───────────────────────────
        var query = _followRepository.GetQueryableNoTracking()
            .Include(f => f.Follower)
            .Include(f => f.Following)
            .Where(f => f.FollowerId == userId)
            .OrderByDescending(f => f.CreatedAt);

        var total = await query.CountAsync(ct);
        var following = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(ct);

        // ── Map to DTO ──────────────────────────────
        var followDtos = following.Select(MapToResponse).ToList();

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
