using Anonwork.Application.Interfaces;
using Anonwork.Domain.Entities;
using Anonwork.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Anonwork.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for Subject entity
/// </summary>
public class SubjectRepository(AppDbContext context) : ISubjectRepository
{
    public async Task<Subject?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await context.Subjects
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == id, ct);
    }

    public async Task<(List<Subject> Subjects, int Total)> GetAllAsync(
        string? searchQuery = null,
        int page = 1,
        int pageSize = 10,
        CancellationToken ct = default)
    {
        // ── Build base query ────────────────────────
        var query = context.Subjects.AsNoTracking();

        // ── Apply search filter ─────────────────────
        if (!string.IsNullOrWhiteSpace(searchQuery))
        {
            var searchTerm = searchQuery.Trim().ToLower();
            query = query.Where(s => 
                s.Name.ToLower().Contains(searchTerm) || 
                s.Slug.ToLower().Contains(searchTerm));
        }

        // ── Get total count ─────────────────────────
        var total = await query.CountAsync(ct);

        // ── Order and paginate ──────────────────────
        var subjects = await query
            .OrderByDescending(s => s.PostCount)
            .ThenByDescending(s => s.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return (subjects, total);
    }

    public async Task<Subject> CreateAsync(Subject subject, CancellationToken ct = default)
    {
        context.Subjects.Add(subject);
        await context.SaveChangesAsync(ct);
        return subject;
    }

    public async Task UpdateAsync(Subject subject, CancellationToken ct = default)
    {
        context.Subjects.Update(subject);
        await context.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var subject = await context.Subjects.FirstOrDefaultAsync(s => s.Id == id, ct);
        if (subject is not null)
        {
            context.Subjects.Remove(subject);
            await context.SaveChangesAsync(ct);
        }
    }

    public async Task<bool> ExistsByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await context.Subjects
            .AnyAsync(s => s.Id == id, ct);
    }

    public async Task<bool> ExistsBySlugAsync(string slug, CancellationToken ct = default)
    {
        var normalizedSlug = slug.Trim().ToLower();
        return await context.Subjects
            .AnyAsync(s => s.Slug.ToLower() == normalizedSlug, ct);
    }

    public async Task<Subject?> GetBySlugAsync(string slug, CancellationToken ct = default)
    {
        var normalizedSlug = slug.Trim().ToLower();
        return await context.Subjects
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Slug.ToLower() == normalizedSlug, ct);
    }
}
