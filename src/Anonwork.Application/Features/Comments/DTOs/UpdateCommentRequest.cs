namespace Anonwork.Application.Features.Comments.DTOs;

/// <summary>
/// Request DTO for updating a comment
/// </summary>
public class UpdateCommentRequest
{
    /// <summary>
    /// Comment content
    /// </summary>
    public string Content { get; set; } = null!;
}
