namespace Anonwork.Application.Features.SubscriptionPlans.DTOs.Responses;

public record SubscriptionPlanResponseDto(
    Guid Id,
    string Name,
    string Slug,
    string? Description,
    long Price,
    int DurationDays,
    int MaxPostsPerDay,
    int MaxUploadsPerDay,
    int MaxPostFileSizeMb,
    int MaxPostImageCount,
    int MaxPostMediaCount,
    bool CanAttachMediaToPost,
    bool CanUploadPostFiles,
    bool CanUseExclusiveAnonImages,
    bool CanUsePremiumFeatures,
    bool IsActive,
    DateTime CreatedAt,
    DateTime? UpdatedAt
);

public record SubscriptionPlanListResponseDto(
    Guid Id,
    string Name,
    string Slug,
    string? Description,
    long Price,
    int DurationDays,
    int MaxPostsPerDay,
    int MaxUploadsPerDay,
    int MaxPostFileSizeMb,
    int MaxPostImageCount,
    int MaxPostMediaCount,
    bool CanAttachMediaToPost,
    bool CanUploadPostFiles,
    bool CanUseExclusiveAnonImages,
    bool CanUsePremiumFeatures,
    bool IsActive,
    DateTime CreatedAt,
    DateTime? UpdatedAt
);

public record SubscriptionPlanListPaginatedResponseDto(
    List<SubscriptionPlanListResponseDto> SubscriptionPlans,
    int Total,
    int Page,
    int PageSize,
    int TotalPages
);
