namespace Anonwork.Application.Features.Posts.DTOs;

/// <summary>
/// DTO for post list response with pagination
/// </summary>
public record PostListResponseDto(
    List<PostResponseDto> Posts,
    int Total,
    int Page,
    int PageSize,
    int TotalPages
);
