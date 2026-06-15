using Anonwork.Application.Common.Exceptions;
using Anonwork.Application.Features.SubscriptionPlans.DTOs.Responses;
using Anonwork.Application.Interfaces;
using Anonwork.Domain.Entities;

namespace Anonwork.Application.Features.SubscriptionPlans;

public class GetSubscriptionPlanByIdUseCase(IUnitOfWork unitOfWork)
{
    private readonly IGenericRepository<SubscriptionPlan> _subscriptionPlanRepo = unitOfWork.GetRepository<SubscriptionPlan>();

    public async Task<SubscriptionPlanResponseDto> ExecuteAsync(Guid id, CancellationToken ct = default)
    {
        var plan = await _subscriptionPlanRepo.GetByIdAsync(id, ct)
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