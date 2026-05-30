using Anonwork.Application.Features.UserSubscriptions.DTOs;
using Anonwork.Application.Interfaces;
using Anonwork.Domain.Entities;

namespace Anonwork.Application.Features.UserSubscriptions;

public class GetUserSubscriptionByIdUseCase(IUnitOfWork unitOfWork)
{
    private readonly IGenericRepository<UserSubscription> _userSubscriptionRepository = unitOfWork.GetRepository<UserSubscription>();
    private readonly IGenericRepository<User> _userRepository = unitOfWork.GetRepository<User>();
    private readonly IGenericRepository<SubscriptionPlan> _subscriptionPlanRepository = unitOfWork.GetRepository<SubscriptionPlan>();

    public async Task<UserSubscriptionResponseDto?> ExecuteAsync(
        Guid id, 
        CancellationToken ct = default)
    {
        var subscription = await _userSubscriptionRepository.GetByIdAsync(id, ct);
        if (subscription == null)
            return null;

        // Get related data for response
        var user = await _userRepository.GetByIdAsync(subscription.UserId, ct);
        var plan = await _subscriptionPlanRepository.GetByIdAsync(subscription.PlanId, ct);

        return new UserSubscriptionResponseDto(
            subscription.Id,
            subscription.UserId,
            subscription.PlanId,
            subscription.OrderId,
            subscription.Status,
            subscription.StartedAt,
            subscription.ExpiresAt,
            subscription.CreatedAt,
            user?.Username,
            plan?.Name
        );
    }
}