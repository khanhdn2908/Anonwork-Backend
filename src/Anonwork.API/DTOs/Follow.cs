namespace Anonwork.API.DTOs;

/// <summary>
/// DTO for creating a follow relationship
/// </summary>
public class CreateFollowDto
{
    /// <summary>
    /// ID of the user to follow
    /// </summary>
    public Guid FollowingId { get; set; }
}

/// <summary>
/// DTO for follow response
/// </summary>
public class FollowDto
{
    /// <summary>
    /// Follow relationship ID
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Follower user ID
    /// </summary>
    public Guid FollowerId { get; set; }

    /// <summary>
    /// Following user ID
    /// </summary>
    public Guid FollowingId { get; set; }

    /// <summary>
    /// Created date
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Follower user details
    /// </summary>
    public UserBasicDto? Follower { get; set; }

    /// <summary>
    /// Following user details
    /// </summary>
    public UserBasicDto? Following { get; set; }
}

/// <summary>
/// DTO for basic user information
/// </summary>
public class UserBasicDto
{
    /// <summary>
    /// User ID
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Username
    /// </summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// User email
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// User avatar URL
    /// </summary>
    public string? Avatar { get; set; }
}

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
    /// Whether the current user is following this user
    /// </summary>
    public bool IsFollowing { get; set; }
}
