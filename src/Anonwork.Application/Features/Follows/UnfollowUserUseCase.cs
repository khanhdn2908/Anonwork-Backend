using Anonwork.Application.Interfaces;

namespace Anonwork.Application.Features.Follows;

/// <summary>
/// Use case for unfollowing a user
/// </summary>
public class UnfollowUserUseCase(IFollowRepository followRepository)
{
    public async Task ExecuteAsync(Guid currentUserId, Guid followingId, CancellationToken ct = default)
    {
        // ── Validate input ──────────────────────────
        if (currentUserId == Guid.Empty)
            throw new ArgumentException("Current user ID is required.");

        if (followingId == Guid.Empty)
            throw new ArgumentException("Following user ID is required.");

        // ── Check if follow relationship exists ─────
        var followExists = await followRepository.ExistsByFollowerAndFollowingAsync(currentUserId, followingId, ct);
        if (!followExists)
            throw new KeyNotFoundException("Follow relationship not found.");

        // ── Delete follow relationship ──────────────
        await followRepository.DeleteByFollowerAndFollowingAsync(currentUserId, followingId, ct);
    }
}
