using Anonwork.Application.Features.UserSubscriptions.DTOs;
using Anonwork.Application.Interfaces;

namespace Anonwork.Application.Features.UserSubscriptions;

public class GetUserSubscriptionsByUserIdUseCase
{
    private readonly IUserSubscriptionRepository _userSubscriptionRepository;
    private readonly IUserRepository _userRepository;
    private readonly ISubscriptionPlanRepository _subscriptionPlanRepository;

    public GetUserSubscriptionsByUserIdUseCase(
        IUserSubscriptionRepository userSubscriptionRepository,
        IUserRepository userRepository,
        ISubscriptionPlanRepository subscriptionPlanRepository)
    {
        _userSubscriptionRepository = userSubscriptionRepository;
        _userRepository = userRepository;
        _subscriptionPlanRepository = subscriptionPlanRepository;
    }

    public async Task<UserSubscriptionListPaginatedResponseDto> ExecuteAsync(
        GetUserSubscriptionsByUserIdRequestDto request, 
        CancellationToken ct = default)
    {
        // Validate user exists
        var user = await _userRepository.GetByIdAsync(request.UserId, ct);
        if (user == null)
            throw new ArgumentException($"User with ID {request.UserId} not found");

        // Get all subscriptions for the user
        var allSubscriptions = await _userSubscriptionRepository.GetByUserIdAsync(request.UserId, ct);
        
        // Apply pagination
        var totalCount = allSubscriptions.Count;
        var totalPages = (int)Math.Ceiling((double)totalCount / request.PageSize);
        
        var paginatedSubscriptions = allSubscriptions
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToList();

        // Get plan information for each subscription
        var subscriptionDtos = new List<UserSubscriptionListResponseDto>();
        
        foreach (var subscription in paginatedSubscriptions)
        {
            var plan = await _subscriptionPlanRepository.GetByIdAsync(subscription.PlanId, ct);
            
            subscriptionDtos.Add(new UserSubscriptionListResponseDto(
                subscription.Id,
                subscription.UserId,
                subscription.PlanId,
                subscription.OrderId,
                subscription.Status,
                subscription.StartedAt,
                subscription.ExpiresAt,
                subscription.CreatedAt,
                user.Username,
                plan?.Name
            ));
        }

        return new UserSubscriptionListPaginatedResponseDto(
            subscriptionDtos,
            totalCount,
            request.Page,
            request.PageSize,
            totalPages
        );
    }
}