namespace Anonwork.Application.Features.Posts.DTOs;

/// <summary>
/// DTO for post response
/// </summary>
public record PostResponseDto(
    Guid Id,
    string Title,
    string Content,
    Guid AuthorId,
    string? AuthorUsername,
    string? AuthorAnonAlias,
    bool IsAnonymous,
    Guid SubjectId,
    string? SubjectName,
    List<string> ImageUrls,
    List<string> Tags,
    int Upvotes,
    int CommentsCount,
    int ViewCount,
    string Status,
    DateTime CreatedAt,
    DateTime UpdatedAt
);
