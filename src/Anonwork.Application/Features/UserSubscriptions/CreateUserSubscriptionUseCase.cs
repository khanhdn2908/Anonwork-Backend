using Anonwork.Application.Features.UserSubscriptions.DTOs;
using Anonwork.Application.Interfaces;
using Anonwork.Domain.Entities;
using Anonwork.Domain.Enums;

namespace Anonwork.Application.Features.UserSubscriptions;

public class CreateUserSubscriptionUseCase
{
    private readonly IUserSubscriptionRepository _userSubscriptionRepository;
    private readonly IUserRepository _userRepository;
    private readonly ISubscriptionPlanRepository _subscriptionPlanRepository;

    public CreateUserSubscriptionUseCase(
        IUserSubscriptionRepository userSubscriptionRepository,
        IUserRepository userRepository,
        ISubscriptionPlanRepository subscriptionPlanRepository)
    {
        _userSubscriptionRepository = userSubscriptionRepository;
        _userRepository = userRepository;
        _subscriptionPlanRepository = subscriptionPlanRepository;
    }

    public async Task<UserSubscriptionResponseDto> ExecuteAsync(
        CreateUserSubscriptionRequestDto request, 
        CancellationToken ct = default)
    {
        // Validate user exists
        var user = await _userRepository.GetByIdAsync(request.UserId, ct);
        if (user == null)
            throw new ArgumentException($"User with ID {request.UserId} not found");

        // Validate subscription plan exists
        var plan = await _subscriptionPlanRepository.GetByIdAsync(request.PlanId, ct);
        if (plan == null)
            throw new ArgumentException($"Subscription plan with ID {request.PlanId} not found");

        // Check if user already has an active subscription
        var hasActiveSubscription = await _userSubscriptionRepository.HasActiveSubscriptionAsync(request.UserId, ct);
        if (hasActiveSubscription)
            throw new InvalidOperationException("User already has an active subscription");

        // Calculate dates
        var startedAt = request.StartedAt ?? DateTime.UtcNow;
        var expiresAt = startedAt.AddDays(plan.DurationDays);

        // Create new subscription
        var subscription = new UserSubscription
        {
            Id = Guid.NewGuid(),
            UserId = request.UserId,
            PlanId = request.PlanId,
            OrderId = request.OrderId,
            Status = request.Status,
            StartedAt = startedAt,
            ExpiresAt = expiresAt,
            CreatedAt = DateTime.UtcNow
        };

        var createdSubscription = await _userSubscriptionRepository.CreateAsync(subscription, ct);

        return new UserSubscriptionResponseDto(
            createdSubscription.Id,
            createdSubscription.UserId,
            createdSubscription.PlanId,
            createdSubscription.OrderId,
            createdSubscription.Status,
            createdSubscription.StartedAt,
            createdSubscription.ExpiresAt,
            createdSubscription.CreatedAt,
            user.Username,
            plan.Name
        );
    }
}