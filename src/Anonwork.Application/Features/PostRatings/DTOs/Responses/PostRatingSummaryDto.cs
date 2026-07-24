using System;
using System.Collections.Generic;

namespace Anonwork.Application.Features.PostRatings.DTOs.Responses;

public record PostRatingItemDto(
    Guid Id,
    Guid UserId,
    string AuthorName,
    int Stars,
    string? Review,
    DateTime CreatedAt,
    DateTime UpdatedAt
);

public record PostRatingSummaryDto(
    Guid PostId,
    decimal AverageRating,
    int RatingsCount,
    double QualityScore,
    Dictionary<int, int> StarBreakdown,
    PostRatingItemDto? MyRating,
    List<PostRatingItemDto> RecentRatings
);
