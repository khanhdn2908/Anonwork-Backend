namespace Anonwork.Application.Features.Posts.DTOs;

/// <summary>
/// Request object for creating a post (internal use)
/// </summary>
public record CreatePostRequest(
    Guid AuthorId,
    string Title,
    string Content,
    Guid SubjectId,
    List<string>? Tags = null,
    List<string>? ImageUrls = null,
    bool IsAnonymous = false
);
