using Anonwork.Application.Common.Exceptions;
using Anonwork.Application.Interfaces;
using Anonwork.Domain.Common.Exceptions;
using Anonwork.Domain.Entities;
using Anonwork.Domain.Enums;

namespace Anonwork.Application.Features.Payments;

public class RenewSubscriptionUseCase(IUnitOfWork unitOfWork)
{
    private readonly IGenericRepository<UserSubscription> _subscriptionRepo = unitOfWork.GetRepository<UserSubscription>();
    private readonly IGenericRepository<SubscriptionPlan> _planRepo = unitOfWork.GetRepository<SubscriptionPlan>();

    public async Task ExecuteAsync(
        Guid subscriptionId,
        CancellationToken ct = default)
    {
        // ── Get subscription ────────────────────────
        var subscription = await _subscriptionRepo.GetByIdAsync(subscriptionId, ct)
            ?? throw new NotFoundException("Subscription not found.");

        // ── Guard: không gia hạn subscription đã cancel
        if (subscription.Status == SubscriptionStatus.Cancelled)
            throw new InvalidOperationException("Cannot renew a cancelled subscription.");

        // ── Get plan ────────────────────────────────
        var plan = await _planRepo.GetByIdAsync(subscription.PlanId, ct)
            ?? throw new NotFoundException("Subscription plan not found.");

        // ── Guard: plan bị deactivate thì không gia hạn
        if (!plan.IsActive)
            throw new InvalidOperationException("Subscription plan is no longer available.");

        // ── Renew — cộng từ ExpiresAt nếu còn hạn, từ now nếu đã hết
        var baseDate = subscription.ExpiresAt > DateTime.UtcNow
            ? subscription.ExpiresAt
            : DateTime.UtcNow;

        subscription.Status = SubscriptionStatus.Active;
        subscription.StartedAt = DateTime.UtcNow;
        subscription.ExpiresAt = baseDate.AddDays(plan.DurationDays);
        //subscription.UpdatedAt = DateTime.UtcNow;

        await _subscriptionRepo.UpdateAsync(subscription, ct);
        await unitOfWork.SaveChangesAsync(ct);
    }
}