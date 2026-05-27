using Anonwork.Application.Common.Exceptions;
using Anonwork.Application.Features.SubscriptionPlans.DTOs;
using Anonwork.Application.Interfaces;
using Anonwork.Domain.Common.Exceptions;

namespace Anonwork.Application.Features.SubscriptionPlans;

public class GetSubscriptionPlanBySlugUseCase(ISubscriptionPlanRepository subscriptionPlanRepo)
{
    public async Task<SubscriptionPlanResponseDto> ExecuteAsync(string slug, CancellationToken ct = default)
    {
        var plan = await subscriptionPlanRepo.GetBySlugAsync(slug, ct)
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