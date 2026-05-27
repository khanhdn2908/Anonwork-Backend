using Anonwork.Domain.Entities;

namespace Anonwork.Application.Interfaces;

public interface IUserSubscriptionRepository
{
    // ── READ ──────────────────────────────────────
    Task<UserSubscription?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<UserSubscription?> GetActiveByUserIdAsync(Guid userId, CancellationToken ct = default);
    Task<List<UserSubscription>> GetByUserIdAsync(Guid userId, CancellationToken ct = default);
    Task<List<UserSubscription>> GetExpiredSubscriptionsAsync(CancellationToken ct = default);

    // ── EXISTS ────────────────────────────────────
    Task<bool> HasActiveSubscriptionAsync(Guid userId, CancellationToken ct = default);

    // ── WRITE ─────────────────────────────────────
    Task<UserSubscription> CreateAsync(UserSubscription subscription, CancellationToken ct = default);
    Task UpdateAsync(UserSubscription subscription, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
}
