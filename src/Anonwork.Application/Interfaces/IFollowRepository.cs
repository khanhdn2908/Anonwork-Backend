using Anonwork.Domain.Entities;

namespace Anonwork.Application.Interfaces;

/// <summary>
/// Repository interface for Follow entity
/// </summary>
public interface IFollowRepository
{
    /// <summary>
    /// Get follow relationship by id
    /// </summary>
    Task<Follow?> GetByIdAsync(Guid id, CancellationToken ct = default);

    /// <summary>
    /// Get follow relationship by follower and following user ids
    /// </summary>
    Task<Follow?> GetByFollowerAndFollowingAsync(Guid followerId, Guid followingId, CancellationToken ct = default);

    /// <summary>
    /// Create a new follow relationship
    /// </summary>
    Task<Follow> CreateAsync(Follow follow, CancellationToken ct = default);

    /// <summary>
    /// Delete a follow relationship
    /// </summary>
    Task DeleteAsync(Guid id, CancellationToken ct = default);

    /// <summary>
    /// Delete follow relationship by follower and following user ids
    /// </summary>
    Task DeleteByFollowerAndFollowingAsync(Guid followerId, Guid followingId, CancellationToken ct = default);

    /// <summary>
    /// Check if a follow relationship exists
    /// </summary>
    Task<bool> ExistsByFollowerAndFollowingAsync(Guid followerId, Guid followingId, CancellationToken ct = default);

    /// <summary>
    /// Get all followers of a user with pagination
    /// </summary>
    Task<(List<Follow> Followers, int Total)> GetFollowersAsync(
        Guid userId,
        int page = 1,
        int pageSize = 10,
        CancellationToken ct = default);

    /// <summary>
    /// Get all users that a user is following with pagination
    /// </summary>
    Task<(List<Follow> Following, int Total)> GetFollowingAsync(
        Guid userId,
        int page = 1,
        int pageSize = 10,
        CancellationToken ct = default);

    /// <summary>
    /// Get follower count for a user
    /// </summary>
    Task<int> GetFollowerCountAsync(Guid userId, CancellationToken ct = default);

    /// <summary>
    /// Get following count for a user
    /// </summary>
    Task<int> GetFollowingCountAsync(Guid userId, CancellationToken ct = default);

    /// <summary>
    /// Check if user A is following user B
    /// </summary>
    Task<bool> IsFollowingAsync(Guid followerId, Guid followingId, CancellationToken ct = default);
}
