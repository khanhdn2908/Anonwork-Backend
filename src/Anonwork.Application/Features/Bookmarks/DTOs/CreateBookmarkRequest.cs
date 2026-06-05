namespace Anonwork.Application.Features.Bookmarks.DTOs;

/// <summary>
/// Request DTO for creating a bookmark
/// </summary>
public class CreateBookmarkRequest
{
    /// <summary>
    /// Bookmarked post ID
    /// </summary>
    public Guid PostId { get; set; }
}