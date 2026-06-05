namespace Anonwork.Application.Features.Bookmarks.DTOs;

/// <summary>
/// DTO for bookmark list response with pagination
/// </summary>
public record BookmarkListResponseDto(
    List<BookmarkResponseDto> Bookmarks,
    int Total,
    int Page,
    int PageSize,
    int TotalPages
);