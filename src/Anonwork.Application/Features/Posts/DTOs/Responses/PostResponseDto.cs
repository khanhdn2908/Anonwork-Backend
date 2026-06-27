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
    string? AuthorAvatarUrl,
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
    bool IsUpvotedByMe
);
