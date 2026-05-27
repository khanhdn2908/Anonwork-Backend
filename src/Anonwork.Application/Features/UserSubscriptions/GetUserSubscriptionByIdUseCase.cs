using Anonwork.Application.Features.UserSubscriptions.DTOs;
using Anonwork.Application.Interfaces;

namespace Anonwork.Application.Features.UserSubscriptions;

public class GetUserSubscriptionByIdUseCase
{
    private readonly IUserSubscriptionRepository _userSubscriptionRepository;
    private readonly IUserRepository _userRepository;
    private readonly ISubscriptionPlanRepository _subscriptionPlanRepository;

    public GetUserSubscriptionByIdUseCase(
        IUserSubscriptionRepository userSubscriptionRepository,
        IUserRepository userRepository,
        ISubscriptionPlanRepository subscriptionPlanRepository)
    {
        _userSubscriptionRepository = userSubscriptionRepository;
        _userRepository = userRepository;
        _subscriptionPlanRepository = subscriptionPlanRepository;
    }

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