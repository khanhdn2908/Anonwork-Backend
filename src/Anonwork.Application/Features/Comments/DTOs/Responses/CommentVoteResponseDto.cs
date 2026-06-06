namespace Anonwork.Application.Features.Comments.DTOs.Responses;

/// <summary>
/// DTO for comment vote response
/// </summary>
public record CommentVoteResponseDto(
    Guid CommentId,
    int Upvotes,
    bool IsUpvoted,
    string Message
);
