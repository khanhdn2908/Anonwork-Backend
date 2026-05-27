using Anonwork.Application.Interfaces;
using Anonwork.Domain.Entities;
using Anonwork.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Anonwork.Infrastructure.Repositories;

public class SubscriptionPlanRepository : ISubscriptionPlanRepository
{
    private readonly AppDbContext _context;

    public SubscriptionPlanRepository(AppDbContext context)
    {
        _context = context;
    }

    // ── READ ──────────────────────────────────────
    public async Task<SubscriptionPlan?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _context.SubscriptionPlans
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id, ct);

    public async Task<SubscriptionPlan?> GetBySlugAsync(string slug, CancellationToken ct = default)
        => await _context.SubscriptionPlans
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Slug == slug, ct);

    public async Task<List<SubscriptionPlan>> GetAllActiveAsync(CancellationToken ct = default)
        => await _context.SubscriptionPlans
            .AsNoTracking()
            .Where(p => p.IsActive)
            .OrderBy(p => p.Price)
            .ToListAsync(ct);

    public async Task<List<SubscriptionPlan>> GetAllAsync(CancellationToken ct = default)
        => await _context.SubscriptionPlans
            .AsNoTracking()
            .OrderBy(p => p.Price)
            .ToListAsync(ct);

    // ── EXISTS ────────────────────────────────────
    public async Task<bool> ExistsByIdAsync(Guid id, CancellationToken ct = default)
        => await _context.SubscriptionPlans
            .AnyAsync(p => p.Id == id, ct);

    // ── WRITE ─────────────────────────────────────
    public async Task<SubscriptionPlan> CreateAsync(SubscriptionPlan plan, CancellationToken ct = default)
    {
        _context.SubscriptionPlans.Add(plan);
        await _context.SaveChangesAsync(ct);
        return plan;
    }

    public async Task UpdateAsync(SubscriptionPlan plan, CancellationToken ct = default)
    {
        _context.SubscriptionPlans.Update(plan);
        await _context.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var plan = await _context.SubscriptionPlans.FirstOrDefaultAsync(p => p.Id == id, ct);
        if (plan is not null)
        {
            _context.SubscriptionPlans.Remove(plan);
            await _context.SaveChangesAsync(ct);
        }
    }
}
