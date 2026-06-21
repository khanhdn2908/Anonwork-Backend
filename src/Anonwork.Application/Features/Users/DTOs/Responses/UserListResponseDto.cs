namespace Anonwork.Application.Features.Users.DTOs.Responses;

public record UserActiveSubscriptionDto(
    Guid Id,
    Guid PlanId,
    string PlanName,
    string PlanSlug,
    long Price,
    int DurationDays,
    DateTime StartedAt,
    DateTime ExpiresAt
);

public record UserListResponseDto(
    Guid Id,
    string Username,
    string? Email,
    string? AvatarUrl,
    string? Bio,
    string AnonAlias,
    bool IsAnonDefault,
    int FollowerCount,
    int FollowingCount,
    bool HasActiveSubscription,
    UserActiveSubscriptionDto? ActiveSubscription,
    DateTime CreatedAt,
    DateTime UpdatedAt
);

public record UserListPaginatedResponseDto(
    List<UserListResponseDto> Users,
    int Total,
    int Page,
    int PageSize,
    int TotalPages
);
