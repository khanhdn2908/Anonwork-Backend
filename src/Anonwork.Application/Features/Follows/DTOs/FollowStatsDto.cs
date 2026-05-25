namespace Anonwork.Application.Features.Follows.DTOs;

/// <summary>
/// DTO for follow statistics
/// </summary>
public class FollowStatsDto
{
    /// <summary>
    /// Number of followers
    /// </summary>
    public int FollowerCount { get; set; }

    /// <summary>
    /// Number of users being followed
    /// </summary>
    public int FollowingCount { get; set; }

    /// <summary>
    /// Whether the current user is following this user (if applicable)
    /// </summary>
    public bool IsFollowing { get; set; }
}
