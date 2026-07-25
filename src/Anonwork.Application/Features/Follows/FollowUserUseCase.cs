using Anonwork.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using Anonwork.Domain.Entities;
using Anonwork.Application.Features.Follows.DTOs.Responses;
using Anonwork.Application.Features.Follows.DTOs.Requests;

namespace Anonwork.Application.Features.Follows;

/// <summary>
/// Use case for following a user
/// </summary>
public class FollowUserUseCase(IUnitOfWork unitOfWork, IActivityLogService activityLogService)
{
    private readonly IGenericRepository<Follow> _followRepository = unitOfWork.GetRepository<Follow>();
    private readonly IGenericRepository<User> _userRepository = unitOfWork.GetRepository<User>();
    private readonly IActivityLogService _activityLogService = activityLogService;

    public async Task<FollowResponseDto> ExecuteAsync(Guid currentUserId, FollowUserRequest request, CancellationToken ct = default)
    {
        // ── Validate input ──────────────────────────
        if (currentUserId == Guid.Empty)
            throw new ArgumentException("Current user ID is required.");

        if (request.FollowingId == Guid.Empty)
            throw new ArgumentException("Following user ID is required.");

        // ── Prevent self-follow ─────────────────────
        if (currentUserId == request.FollowingId)
            throw new InvalidOperationException("You cannot follow yourself.");

        // ── Check if following user exists ──────────
        var followingUserExists = await _userRepository.ExistsAsync(request.FollowingId, ct);
        if (!followingUserExists)
            throw new KeyNotFoundException("User to follow not found.");

        // ── Check if already following ──────────────
        var alreadyFollowing = await _followRepository.ExistsAsync(f => f.FollowerId == currentUserId && f.FollowingId == request.FollowingId, ct);
        if (alreadyFollowing)
            throw new InvalidOperationException("You are already following this user.");

        // ── Create follow relationship ──────────────
        var follow = new Follow
        {
            Id = Guid.NewGuid(),
            FollowerId = currentUserId,
            FollowingId = request.FollowingId,
            CreatedAt = DateTime.UtcNow,
        };

        var createdFollow = await _followRepository.AddAsync(follow, ct);
        await unitOfWork.SaveChangesAsync(ct);

        _ = _activityLogService.LogAsync(
            currentUserId,
            "FOLLOW_USER",
            "User",
            $"Theo dõi người dùng ID '{request.FollowingId}'",
            "user",
            request.FollowingId.ToString(),
            ct: ct);

        // ── Load follow relationship with user data ─
        var followWithUsers = await _followRepository.GetQueryableNoTracking()
            .Include(f => f.Follower)
            .Include(f => f.Following)
            .FirstOrDefaultAsync(f => f.Id == createdFollow.Id, ct);
        if (followWithUsers == null)
            throw new InvalidOperationException("Failed to retrieve created follow relationship.");

        // ── Map to DTO ──────────────────────────────
        return MapToResponse(followWithUsers);
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
                AvatarUrl = follow.Follower.AvatarKey
            } : null,
            Following = follow.Following != null ? new UserBasicDto
            {
                Id = follow.Following.Id,
                Username = follow.Following.Username,
                Email = follow.Following.Email,
                AvatarUrl = follow.Following.AvatarKey
            } : null
        };
    }
}

