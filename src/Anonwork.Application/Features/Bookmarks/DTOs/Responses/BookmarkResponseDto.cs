namespace Anonwork.Application.Features.Bookmarks.DTOs.Responses;

/// <summary>
/// DTO for bookmark response
/// </summary>
public record BookmarkResponseDto(
    Guid Id,
    Guid UserId,
    Guid PostId,
    DateTime CreatedAt,
    BookmarkPostDto? Post = null
);

/// <summary>
/// DTO for bookmarked post preview
/// </summary>
public record BookmarkPostDto(
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
    int RemainingImagesCount,
    List<string> Tags,
    int Upvotes,
    int CommentsCount,
    int ViewCount,
    string Status,
    DateTime CreatedAt,
    DateTime UpdatedAt
);