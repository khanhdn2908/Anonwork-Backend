using Anonwork.Application.Features.SubscriptionPlans.DTOs.Requests;
using Anonwork.Application.Features.SubscriptionPlans.DTOs.Responses;
using Anonwork.Application.Interfaces;
using Anonwork.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Anonwork.Application.Features.SubscriptionPlans;

public class GetAllSubscriptionPlansUseCase(IUnitOfWork unitOfWork)
{
    private readonly IGenericRepository<SubscriptionPlan> _subscriptionPlanRepo = unitOfWork.GetRepository<SubscriptionPlan>();

    public async Task<SubscriptionPlanListPaginatedResponseDto> ExecuteAsync(
        GetAllSubscriptionPlansRequestDto request,
        CancellationToken ct = default)
    {
        // Validate pagination
        var page = request.Page < 1 ? 1 : request.Page;
        var pageSize = request.PageSize < 1 ? 10 : request.PageSize;
        if (pageSize > 100) pageSize = 100; // Max 100 per page

        var query = _subscriptionPlanRepo.GetQueryableNoTracking();

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var term = request.SearchTerm.Trim().ToLower();
            query = query.Where(p => p.Name.ToLower().Contains(term) || p.Slug.ToLower().Contains(term));
        }

        if (request.IsActive.HasValue)
        {
            query = query.Where(p => p.IsActive == request.IsActive.Value);
        }

        var total = await query.CountAsync(ct);
        var plans = await query
            .OrderByDescending(p => p.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

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