namespace Anonwork.Application.Features.Follows.DTOs.Responses;

/// <summary>
/// Response DTO for follow relationship
/// </summary>
public class FollowResponseDto
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
    /// Follower user information
    /// </summary>
    public UserBasicDto? Follower { get; set; }

    /// <summary>
    /// Following user information
    /// </summary>
    public UserBasicDto? Following { get; set; }

    /// <summary>
    /// Created timestamp
    /// </summary>
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// Basic user information DTO
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
    /// Email
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Avatar URL
    /// </summary>
    public string? AvatarUrl { get; set; }
}
