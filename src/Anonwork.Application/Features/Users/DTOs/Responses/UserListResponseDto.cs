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
    string? AvatarUrl,
    string? Bio,
    string AnonAlias,
    DateTime CreatedAt,
    bool HasActiveSubscription,
    UserActiveSubscriptionDto? ActiveSubscription
);

public record UserListPaginatedResponseDto(
    List<UserListResponseDto> Users,
    int Total,
    int Page,
    int PageSize,
    int TotalPages
);
