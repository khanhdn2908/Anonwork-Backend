using Anonwork.Application.Interfaces;

namespace Anonwork.Application.Features.Follows;

/// <summary>
/// Use case for checking if a user is following another user
/// </summary>
public class IsFollowingUseCase(IFollowRepository followRepository)
{
    public async Task<bool> ExecuteAsync(Guid followerId, Guid followingId, CancellationToken ct = default)
    {
        // ── Validate input ──────────────────────────
        if (followerId == Guid.Empty)
            throw new ArgumentException("Follower ID is required.");

        if (followingId == Guid.Empty)
            throw new ArgumentException("Following ID is required.");

        // ── Check if following ──────────────────────
        return await followRepository.IsFollowingAsync(followerId, followingId, ct);
    }
}
