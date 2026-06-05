namespace Anonwork.Application.Features.Comments.DTOs;

/// <summary>
/// DTO for comment response
/// </summary>
public record CommentResponseDto(
    Guid Id,
    Guid PostId,
    Guid AuthorId,
    Guid? ParentId,
    bool IsAnonymous,
    string Content,
    int Upvotes,
    int Depth,
    bool IsDeleted,
    DateTime? DeletedAt,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    CommentAuthorDto? Author = null,
    CommentParentDto? Parent = null,
    List<CommentResponseDto>? Replies = null
);

/// <summary>
/// DTO for comment author preview
/// </summary>
public record CommentAuthorDto(
    Guid Id,
    string Username,
    string? AnonAlias,
    string? AvatarUrl
);

/// <summary>
/// DTO for parent comment preview
/// </summary>
public record CommentParentDto(
    Guid Id,
    string Content,
    Guid AuthorId,
    bool IsAnonymous,
    DateTime CreatedAt
);
