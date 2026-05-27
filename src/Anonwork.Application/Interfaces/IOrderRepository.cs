using Anonwork.Domain.Entities;

namespace Anonwork.Application.Interfaces;

public interface IOrderRepository
{
    // ── READ ──────────────────────────────────────
    Task<Order?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<Order?> GetByOrderCodeAsync(string orderCode, CancellationToken ct = default);
    Task<Order?> GetBySepayTransactionIdAsync(string transactionId, CancellationToken ct = default);
    Task<List<Order>> GetByUserIdAsync(Guid userId, CancellationToken ct = default);
    Task<List<Order>> GetPendingOrdersAsync(CancellationToken ct = default);

    // ── EXISTS ────────────────────────────────────
    Task<bool> ExistsByOrderCodeAsync(string orderCode, CancellationToken ct = default);

    // ── WRITE ─────────────────────────────────────
    Task<Order> CreateAsync(Order order, CancellationToken ct = default);
    Task UpdateAsync(Order order, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
}
