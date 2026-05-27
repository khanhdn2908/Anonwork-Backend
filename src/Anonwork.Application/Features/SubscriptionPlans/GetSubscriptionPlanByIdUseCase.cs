using Anonwork.Application.Common.Exceptions;
using Anonwork.Application.Features.SubscriptionPlans.DTOs;
using Anonwork.Application.Interfaces;
using Anonwork.Domain.Common.Exceptions;

namespace Anonwork.Application.Features.SubscriptionPlans;

public class GetSubscriptionPlanByIdUseCase(ISubscriptionPlanRepository subscriptionPlanRepo)
{
    public async Task<SubscriptionPlanResponseDto> ExecuteAsync(Guid id, CancellationToken ct = default)
    {
        var plan = await subscriptionPlanRepo.GetByIdAsync(id, ct)
            ?? throw new NotFoundException($"Subscription plan with ID {id} not found.");

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