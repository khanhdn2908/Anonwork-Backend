using Anonwork.Application.Interfaces;
using Anonwork.Domain.Enums;

namespace Anonwork.Application.Features.UserSubscriptions;

public class DeleteUserSubscriptionUseCase
{
    private readonly IUserSubscriptionRepository _userSubscriptionRepository;

    public DeleteUserSubscriptionUseCase(IUserSubscriptionRepository userSubscriptionRepository)
    {
        _userSubscriptionRepository = userSubscriptionRepository;
    }

    public async Task<bool> ExecuteAsync(Guid id, CancellationToken ct = default)
    {
        // Check if subscription exists
        var subscription = await _userSubscriptionRepository.GetByIdAsync(id, ct);
        if (subscription == null)
            return false;

        // Business rule: Only allow deletion of non-active subscriptions
        // Active subscriptions should be cancelled instead of deleted
        if (subscription.Status == SubscriptionStatus.Active)
        {
            throw new InvalidOperationException(
                "Cannot delete an active subscription. Please cancel it first using the update endpoint.");
        }

        // Delete the subscription
        await _userSubscriptionRepository.DeleteAsync(id, ct);
        return true;
    }
}