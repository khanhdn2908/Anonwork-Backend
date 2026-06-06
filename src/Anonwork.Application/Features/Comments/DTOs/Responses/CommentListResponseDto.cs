namespace Anonwork.Application.Features.Comments.DTOs.Responses;

/// <summary>
/// DTO for comment list response with pagination
/// </summary>
public record CommentListResponseDto(
    List<CommentResponseDto> Comments,
    int Total,
    int Page,
    int PageSize,
    int TotalPages
);
