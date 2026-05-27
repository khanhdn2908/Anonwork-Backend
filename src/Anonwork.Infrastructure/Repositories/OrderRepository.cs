using Anonwork.Application.Interfaces;
using Anonwork.Domain.Entities;
using Anonwork.Domain.Enums;
using Anonwork.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Anonwork.Infrastructure.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly AppDbContext _context;

    public OrderRepository(AppDbContext context)
    {
        _context = context;
    }

    // ── READ ──────────────────────────────────────
    public async Task<Order?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _context.Orders
            .AsNoTracking()
            .FirstOrDefaultAsync(o => o.Id == id, ct);

    public async Task<Order?> GetByOrderCodeAsync(string orderCode, CancellationToken ct = default)
        => await _context.Orders
            .AsNoTracking()
            .FirstOrDefaultAsync(o => o.OrderCode == orderCode, ct);

    public async Task<Order?> GetBySepayTransactionIdAsync(string transactionId, CancellationToken ct = default)
        => await _context.Orders
            .AsNoTracking()
            .FirstOrDefaultAsync(o => o.ProviderTransactionId == transactionId, ct);

    public async Task<List<Order>> GetByUserIdAsync(Guid userId, CancellationToken ct = default)
        => await _context.Orders
            .AsNoTracking()
            .Where(o => o.UserId == userId)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync(ct);

    public async Task<List<Order>> GetPendingOrdersAsync(CancellationToken ct = default)
        => await _context.Orders
            .AsNoTracking()
            .Where(o => o.Status == OrderStatus.Pending && o.ExpiresAt < DateTime.UtcNow)
            .ToListAsync(ct);

    // ── EXISTS ────────────────────────────────────
    public async Task<bool> ExistsByOrderCodeAsync(string orderCode, CancellationToken ct = default)
        => await _context.Orders
            .AnyAsync(o => o.OrderCode == orderCode, ct);

    // ── WRITE ─────────────────────────────────────
    public async Task<Order> CreateAsync(Order order, CancellationToken ct = default)
    {
        _context.Orders.Add(order);
        await _context.SaveChangesAsync(ct);
        return order;
    }

    public async Task UpdateAsync(Order order, CancellationToken ct = default)
    {
        _context.Orders.Update(order);
        await _context.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == id, ct);
        if (order is not null)
        {
            _context.Orders.Remove(order);
            await _context.SaveChangesAsync(ct);
        }
    }
}
