using Anonwork.Application.Features.AnonImages.DTOs.Responses;
using Anonwork.Application.Features.UserSubscriptions.DTOs.Responses;

namespace Anonwork.Application.Features.Users.DTOs.Responses;

public record GetMeResponseDto(
    Guid Id,
    string Username,
    string Email,
    string? AvatarKey,
    string? AvatarUrl,
    string? Bio,
    string AnonAlias,
    bool IsAnonDefault,
    int FollowerCount,
    int FollowingCount,
    string? AnonImageUrl,
    List<string> UserSubscriptionPlanActive,
    DateTime CreatedAt,
    DateTime UpdatedAt
);
