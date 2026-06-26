namespace Anonwork.Application.Features.Posts.DTOs.Response;

/// <summary>
/// DTO for post response
/// </summary>
public record PostResponseDto(
    Guid Id,
    string Title,
    string Content,
    Guid AuthorId,
    string? AuthorUsername,
    bool IsAnonymous,
    Guid SubjectId,
    string? SubjectName,
    List<PostMediaResponseDto> Media,
    List<string> Tags,
    int Upvotes,
    int CommentsCount,
    int ViewCount,
    string Status,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    bool IsUpvotedByMe,
    // For anonymous posts this is the author's anonymous-image URL; for normal
    // posts it's the author's avatar URL. Null when none is set.
    string? AuthorAvatarUrl = null
);
