using Anonwork.Application.Common.Exceptions;
using Anonwork.Application.Features.SubscriptionPlans.DTOs.Responses;
using Anonwork.Application.Interfaces;
using Anonwork.Domain.Entities;

namespace Anonwork.Application.Features.SubscriptionPlans;

public class GetSubscriptionPlanBySlugUseCase(IUnitOfWork unitOfWork)
{
    private readonly IGenericRepository<SubscriptionPlan> _subscriptionPlanRepo = unitOfWork.GetRepository<SubscriptionPlan>();

    public async Task<SubscriptionPlanResponseDto> ExecuteAsync(string slug, CancellationToken ct = default)
    {
        var normalized = slug.Trim().ToLowerInvariant();
        var plan = await _subscriptionPlanRepo.FindSingleAsync(p => p.Slug == normalized, ct)
            ?? throw new NotFoundException($"Subscription plan with slug '{slug}' not found.");

        return new SubscriptionPlanResponseDto(
            plan.Id,
            plan.Name,
            plan.Slug,
            plan.Price,
            plan.DurationDays,
            plan.Features,
            plan.IsActive,
            plan.CreatedAt
        );
    }
}