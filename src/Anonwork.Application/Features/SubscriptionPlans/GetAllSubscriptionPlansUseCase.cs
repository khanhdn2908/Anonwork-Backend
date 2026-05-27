using Anonwork.Application.Features.SubscriptionPlans.DTOs;
using Anonwork.Application.Interfaces;

namespace Anonwork.Application.Features.SubscriptionPlans;

public class GetAllSubscriptionPlansUseCase(ISubscriptionPlanRepository subscriptionPlanRepo)
{
    public async Task<SubscriptionPlanListPaginatedResponseDto> ExecuteAsync(
        GetAllSubscriptionPlansRequestDto request,
        CancellationToken ct = default)
    {
        // Validate pagination
        var page = request.Page < 1 ? 1 : request.Page;
        var pageSize = request.PageSize < 1 ? 10 : request.PageSize;
        if (pageSize > 100) pageSize = 100; // Max 100 per page

        var (plans, total) = await subscriptionPlanRepo.GetAllAsync(
            request.SearchTerm,
            request.IsActive,
            page,
            pageSize,
            ct);

        var planDtos = plans.Select(p => new SubscriptionPlanListResponseDto(
            p.Id,
            p.Name,
            p.Slug,
            p.Price,
            p.DurationDays,
            p.Features,
            p.IsActive,
            p.CreatedAt
        )).ToList();

        var totalPages = (int)Math.Ceiling(total / (double)pageSize);

        return new SubscriptionPlanListPaginatedResponseDto(
            planDtos,
            total,
            page,
            pageSize,
            totalPages);
    }
}