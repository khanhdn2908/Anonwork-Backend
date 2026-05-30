using Anonwork.Application.Interfaces;
using Anonwork.Domain.Enums;

namespace Anonwork.Application.Features.UserSubscriptions;

public class DeleteUserSubscriptionUseCase(IUnitOfWork unitOfWork)
{
    private readonly IGenericRepository<Anonwork.Domain.Entities.UserSubscription> _userSubscriptionRepository = unitOfWork.GetRepository<Anonwork.Domain.Entities.UserSubscription>();

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
        await unitOfWork.SaveChangesAsync(ct);
        return true;
    }
}