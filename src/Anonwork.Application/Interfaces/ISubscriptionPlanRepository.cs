using Anonwork.Domain.Entities;

namespace Anonwork.Application.Interfaces;

public interface ISubscriptionPlanRepository
{
    // ── READ ──────────────────────────────────────
    Task<SubscriptionPlan?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<SubscriptionPlan?> GetBySlugAsync(string slug, CancellationToken ct = default);
    Task<List<SubscriptionPlan>> GetAllActiveAsync(CancellationToken ct = default);
    Task<List<SubscriptionPlan>> GetAllAsync(CancellationToken ct = default);

    // ── EXISTS ────────────────────────────────────
    Task<bool> ExistsByIdAsync(Guid id, CancellationToken ct = default);

    // ── WRITE ─────────────────────────────────────
    Task<SubscriptionPlan> CreateAsync(SubscriptionPlan plan, CancellationToken ct = default);
    Task UpdateAsync(SubscriptionPlan plan, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
}
