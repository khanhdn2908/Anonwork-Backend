using Anonwork.Domain.Enums;

namespace Anonwork.Application.Features.UserSubscriptions.DTOs;

public record UserSubscriptionResponseDto(
    Guid Id,
    Guid UserId,
    Guid PlanId,
    Guid OrderId,
    SubscriptionStatus Status,
    DateTime StartedAt,
    DateTime ExpiresAt,
    DateTime CreatedAt,
    string? UserName = null,
    string? PlanName = null
);

public record UserSubscriptionListResponseDto(
    Guid Id,
    Guid UserId,
    Guid PlanId,
    Guid OrderId,
    SubscriptionStatus Status,
    DateTime StartedAt,
    DateTime ExpiresAt,
    DateTime CreatedAt,
    string? UserName = null,
    string? PlanName = null
);

public record UserSubscriptionListPaginatedResponseDto(
    List<UserSubscriptionListResponseDto> Items,
    int TotalCount,
    int Page,
    int PageSize,
    int TotalPages
);