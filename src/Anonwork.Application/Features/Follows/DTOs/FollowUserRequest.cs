namespace Anonwork.Application.Features.Follows.DTOs;

/// <summary>
/// Request DTO for following a user
/// </summary>
public class FollowUserRequest
{
    /// <summary>
    /// ID of the user to follow
    /// </summary>
    public Guid FollowingId { get; set; }
}
