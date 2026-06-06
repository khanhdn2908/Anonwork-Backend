namespace Anonwork.Application.Features.Posts.DTOs.Request;

/// <summary>
/// Request object for updating a post (internal use)
/// </summary>
public record UpdatePostRequest(
    Guid PostId,
    Guid AuthorId,
    string? Title = null,
    string? Content = null,
    List<string>? Tags = null,
    List<string>? NewImageUrls = null,
    List<string>? RemoveImageUrls = null
);
