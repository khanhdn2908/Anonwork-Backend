namespace Anonwork.Application.Features.Comments.DTOs.Requests;

/// <summary>
/// Request DTO for creating a comment
/// </summary>
public class CreateCommentRequest
{
    /// <summary>
    /// Post ID to comment on
    /// </summary>
    public Guid PostId { get; set; }

    /// <summary>
    /// Optional parent comment ID for replies
    /// </summary>
    public Guid? ParentId { get; set; }

    /// <summary>
    /// Comment content
    /// </summary>
    public string Content { get; set; } = null!;

    /// <summary>
    /// Whether this comment is anonymous
    /// </summary>
    public bool IsAnonymous { get; set; }
}
