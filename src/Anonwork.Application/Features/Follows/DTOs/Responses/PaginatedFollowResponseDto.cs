namespace Anonwork.Application.Features.Follows.DTOs.Responses;

/// <summary>
/// Paginated response DTO for follow relationships
/// </summary>
public class PaginatedFollowResponseDto
{
    /// <summary>
    /// List of follow relationships
    /// </summary>
    public List<FollowResponseDto> Data { get; set; } = new();

    /// <summary>
    /// Current page number
    /// </summary>
    public int Page { get; set; }

    /// <summary>
    /// Number of items per page
    /// </summary>
    public int PageSize { get; set; }

    /// <summary>
    /// Total number of items
    /// </summary>
    public int Total { get; set; }

    /// <summary>
    /// Total number of pages
    /// </summary>
    public int TotalPages { get; set; }
}
