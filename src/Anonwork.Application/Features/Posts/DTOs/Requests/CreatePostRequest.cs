using Microsoft.AspNetCore.Http;

namespace Anonwork.Application.Features.Posts.DTOs.Request;

/// <summary>
/// Request object for creating a post (internal use)
/// </summary>
public record CreatePostRequest(
    Guid AuthorId,
    string Title,
    string Content,
    Guid SubjectId,
    List<string>? Tags = null,
    IFormFileCollection? Images = null,
    IFormFileCollection? File = null,
    bool IsAnonymous = false
);
