namespace Anonwork.Application.Features.Users.DTOs.Responses;

public record UserResponseDto(
    Guid Id,
    string Username,
    string? Email,
    string? AvatarKey,
    string? Bio,
    string AnonAlias,
    bool IsAnonDefault,
    int FollowerCount,
    int FollowingCount,
    List<string> UserSubscriptionPlanActive,
    DateTime CreatedAt,
    DateTime UpdatedAt
);
