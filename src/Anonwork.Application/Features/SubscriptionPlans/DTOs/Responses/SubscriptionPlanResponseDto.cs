namespace Anonwork.Application.Features.SubscriptionPlans.DTOs.Responses;

public record SubscriptionPlanResponseDto(
    Guid Id,
    string Name,
    string Slug,
    long Price,
    int DurationDays,
    string? Features,
    bool IsActive,
    DateTime CreatedAt
);

public record SubscriptionPlanListResponseDto(
    Guid Id,
    string Name,
    string Slug,
    long Price,
    int DurationDays,
    string? Features,
    bool IsActive,
    DateTime CreatedAt
);

public record SubscriptionPlanListPaginatedResponseDto(
    List<SubscriptionPlanListResponseDto> SubscriptionPlans,
    int Total,
    int Page,
    int PageSize,
    int TotalPages
);