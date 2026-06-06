using Anonwork.Application.Features.UserSubscriptions.DTOs.Requests;
using Anonwork.Application.Features.UserSubscriptions.DTOs.Responses;
using Anonwork.Application.Interfaces;
using Anonwork.Domain.Entities;

namespace Anonwork.Application.Features.UserSubscriptions;

public class UpdateUserSubscriptionUseCase(IUnitOfWork unitOfWork)
{
    private readonly IGenericRepository<UserSubscription> _userSubscriptionRepository = unitOfWork.GetRepository<UserSubscription>();
    private readonly IGenericRepository<User> _userRepository = unitOfWork.GetRepository<User>();
    private readonly IGenericRepository<SubscriptionPlan> _subscriptionPlanRepository = unitOfWork.GetRepository<SubscriptionPlan>();

    public async Task<UserSubscriptionResponseDto> ExecuteAsync(
        Guid id,
        UpdateUserSubscriptionRequestDto request, 
        CancellationToken ct = default)
    {
        // Get existing subscription
        var subscription = await _userSubscriptionRepository.GetByIdAsync(id, ct);
        if (subscription == null)
            throw new ArgumentException($"User subscription with ID {id} not found");

        // Update only provided fields
        if (request.Status.HasValue)
        {
            subscription.Status = request.Status.Value;
        }

        if (request.ExpiresAt.HasValue)
        {
            subscription.ExpiresAt = request.ExpiresAt.Value;
        }

        // Save changes
        await _userSubscriptionRepository.UpdateAsync(subscription, ct);
        await unitOfWork.SaveChangesAsync(ct);

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