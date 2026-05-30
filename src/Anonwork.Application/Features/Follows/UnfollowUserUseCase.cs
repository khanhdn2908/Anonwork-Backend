using Anonwork.Application.Interfaces;
using Anonwork.Domain.Entities;

namespace Anonwork.Application.Features.Follows;

/// <summary>
/// Use case for unfollowing a user
/// </summary>
public class UnfollowUserUseCase(IUnitOfWork unitOfWork)
{
    private readonly IGenericRepository<Follow> _followRepository = unitOfWork.GetRepository<Follow>();

    public async Task ExecuteAsync(Guid currentUserId, Guid followingId, CancellationToken ct = default)
    {
        // ── Validate input ──────────────────────────
        if (currentUserId == Guid.Empty)
            throw new ArgumentException("Current user ID is required.");

        if (followingId == Guid.Empty)
            throw new ArgumentException("Following user ID is required.");

        // ── Check if follow relationship exists ─────
        var follow = await _followRepository.FindSingleAsync(f => f.FollowerId == currentUserId && f.FollowingId == followingId, ct);
        if (follow == null)
            throw new KeyNotFoundException("Follow relationship not found.");

        // ── Delete follow relationship ──────────────
        await _followRepository.DeleteAsync(follow.Id, ct);
        await unitOfWork.SaveChangesAsync(ct);
    }
}
