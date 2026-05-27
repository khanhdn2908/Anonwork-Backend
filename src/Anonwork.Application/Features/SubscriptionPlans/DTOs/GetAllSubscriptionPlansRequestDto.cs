namespace Anonwork.Application.Features.SubscriptionPlans.DTOs;

public record GetAllSubscriptionPlansRequestDto(
    string? SearchTerm = null,
    bool? IsActive = null,
    int Page = 1,
    int PageSize = 10
);