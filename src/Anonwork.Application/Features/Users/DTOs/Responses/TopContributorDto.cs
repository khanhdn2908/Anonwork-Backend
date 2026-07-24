using System;
using System.Collections.Generic;

namespace Anonwork.Application.Features.Users.DTOs.Responses;

public record TopContributorDto(
    int Rank,
    Guid UserId,
    string Username,
    string DisplayName,
    string? AvatarUrl,
    bool IsAnonymous,
    int PostsCount,
    int CommentsCount,
    int UpvotesReceived,
    decimal AverageRating,
    double ContributionScore
);

public record TopContributorsListResponseDto(
    int Month,
    int Year,
    List<TopContributorDto> Contributors
);
