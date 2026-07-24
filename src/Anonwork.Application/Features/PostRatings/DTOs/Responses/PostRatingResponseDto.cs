using System;

namespace Anonwork.Application.Features.PostRatings.DTOs.Responses;

public record PostRatingResponseDto(
    Guid PostId,
    decimal AverageRating,
    int RatingsCount,
    double QualityScore,
    int MyStars,
    string? MyReview,
    string Message
);
