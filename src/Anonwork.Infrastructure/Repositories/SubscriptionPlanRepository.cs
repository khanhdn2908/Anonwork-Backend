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

    public async Task<(List<SubscriptionPlan> plans, int total)> GetAllAsync(
        string? searchTerm = null,
        bool? isActive = null,
        int page = 1,
        int pageSize = 10,
        CancellationToken ct = default)
    {
        var query = _context.SubscriptionPlans.AsNoTracking();

        // Apply search filter
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(p => 
                p.Name.Contains(searchTerm) || 
                p.Slug.Contains(searchTerm));
        }

        // Apply active filter
        if (isActive.HasValue)
        {
            query = query.Where(p => p.IsActive == isActive.Value);
        }

        // Get total count before pagination
        var total = await query.CountAsync(ct);

        // Apply pagination and ordering
        var plans = await query
            .OrderBy(p => p.Price)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return (plans, total);
    }

    // ── EXISTS ────────────────────────────────────
    public async Task<bool> ExistsByIdAsync(Guid id, CancellationToken ct = default)
        => await _context.SubscriptionPlans
            .AnyAsync(p => p.Id == id, ct);

    public async Task<bool> ExistsBySlugAsync(string slug, CancellationToken ct = default)
        => await _context.SubscriptionPlans
            .AnyAsync(p => p.Slug == slug, ct);

    public async Task<bool> ExistsBySlugAsync(string slug, Guid excludeId, CancellationToken ct = default)
        => await _context.SubscriptionPlans
            .AnyAsync(p => p.Slug == slug && p.Id != excludeId, ct);

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
