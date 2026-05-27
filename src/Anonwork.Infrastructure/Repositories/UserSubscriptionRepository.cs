using Anonwork.Application.Interfaces;
using Anonwork.Domain.Entities;
using Anonwork.Domain.Enums;
using Anonwork.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Anonwork.Infrastructure.Repositories;

public class UserSubscriptionRepository : IUserSubscriptionRepository
{
    private readonly AppDbContext _context;

    public UserSubscriptionRepository(AppDbContext context)
    {
        _context = context;
    }

    // ── READ ──────────────────────────────────────
    public async Task<UserSubscription?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _context.UserSubscriptions
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == id, ct);

    public async Task<UserSubscription?> GetActiveByUserIdAsync(Guid userId, CancellationToken ct = default)
        => await _context.UserSubscriptions
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.UserId == userId && s.Status == SubscriptionStatus.Active, ct);

    public async Task<List<UserSubscription>> GetByUserIdAsync(Guid userId, CancellationToken ct = default)
        => await _context.UserSubscriptions
            .AsNoTracking()
            .Where(s => s.UserId == userId)
            .OrderByDescending(s => s.CreatedAt)
            .ToListAsync(ct);

    public async Task<List<UserSubscription>> GetExpiredSubscriptionsAsync(CancellationToken ct = default)
        => await _context.UserSubscriptions
            .AsNoTracking()
            .Where(s => s.Status == SubscriptionStatus.Active && s.ExpiresAt < DateTime.UtcNow)
            .ToListAsync(ct);

    // ── EXISTS ────────────────────────────────────
    public async Task<bool> HasActiveSubscriptionAsync(Guid userId, CancellationToken ct = default)
        => await _context.UserSubscriptions
            .AnyAsync(s => s.UserId == userId && s.Status == SubscriptionStatus.Active, ct);

    // ── WRITE ─────────────────────────────────────
    public async Task<UserSubscription> CreateAsync(UserSubscription subscription, CancellationToken ct = default)
    {
        _context.UserSubscriptions.Add(subscription);
        await _context.SaveChangesAsync(ct);
        return subscription;
    }

    public async Task UpdateAsync(UserSubscription subscription, CancellationToken ct = default)
    {
        _context.UserSubscriptions.Update(subscription);
        await _context.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var subscription = await _context.UserSubscriptions.FirstOrDefaultAsync(s => s.Id == id, ct);
        if (subscription is not null)
        {
            _context.UserSubscriptions.Remove(subscription);
            await _context.SaveChangesAsync(ct);
        }
    }
}
