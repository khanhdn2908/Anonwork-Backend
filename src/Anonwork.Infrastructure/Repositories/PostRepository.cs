using Anonwork.Application.Interfaces;
using Anonwork.Domain.Entities;
using Anonwork.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Anonwork.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for Post entity
/// </summary>
public class PostRepository(AppDbContext context) : IPostRepository
{
    public async Task<Post?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await context.Posts
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id && p.Status == "active", ct);
    }

    public async Task<Post?> GetByIdWithDetailsAsync(Guid id, CancellationToken ct = default)
    {
        return await context.Posts
            .AsNoTracking()
            .Include(p => p.Author)
            .Include(p => p.Subject)
            .Include(p => p.PostImages.OrderBy(pi => pi.DisplayOrder))
            .Include(p => p.PostTags)
            .FirstOrDefaultAsync(p => p.Id == id && p.Status == "active", ct);
    }

    public async Task<Post> CreateAsync(Post post, CancellationToken ct = default)
    {
        context.Posts.Add(post);
        await context.SaveChangesAsync(ct);
        return post;
    }

    public async Task UpdateAsync(Post post, CancellationToken ct = default)
    {
        context.Posts.Update(post);
        await context.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var post = await context.Posts.FirstOrDefaultAsync(p => p.Id == id, ct);
        if (post is not null)
        {
            post.Status = "removed";  // ✅ Changed from "deleted" to "removed"
            post.DeletedAt = DateTime.UtcNow;
            context.Posts.Update(post);
            await context.SaveChangesAsync(ct);
        }
    }

    public async Task<bool> ExistsByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await context.Posts
            .AnyAsync(p => p.Id == id && p.Status == "active", ct);
    }

    public async Task<(List<Post> Posts, int Total)> GetBySubjectAsync(
        Guid subjectId,
        int page = 1,
        int pageSize = 10,
        CancellationToken ct = default)
    {
        var query = context.Posts
            .AsNoTracking()
            .Where(p => p.SubjectId == subjectId && p.Status == "active")
            .OrderByDescending(p => p.CreatedAt);

        var total = await query.CountAsync(ct);

        var posts = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Include(p => p.Author)
            .Include(p => p.Subject)
            .Include(p => p.PostImages.OrderBy(pi => pi.DisplayOrder))
            .Include(p => p.PostTags)
            .ToListAsync(ct);

        return (posts, total);
    }

    public async Task<(List<Post> Posts, int Total)> GetAllAsync(
        int page = 1,
        int pageSize = 10,
        CancellationToken ct = default)
    {
        var query = context.Posts
            .AsNoTracking()
            .Where(p => p.Status == "active")
            .OrderByDescending(p => p.CreatedAt);

        var total = await query.CountAsync(ct);

        var posts = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Include(p => p.Author)
            .Include(p => p.Subject)
            .Include(p => p.PostImages.OrderBy(pi => pi.DisplayOrder))
            .Include(p => p.PostTags)
            .ToListAsync(ct);

        return (posts, total);
    }

    public async Task<List<Post>> GetByAuthorAsync(Guid authorId, CancellationToken ct = default)
    {
        return await context.Posts
            .AsNoTracking()
            .Where(p => p.AuthorId == authorId && p.Status == "active")
            .OrderByDescending(p => p.CreatedAt)
            .Include(p => p.Author)
            .Include(p => p.Subject)
            .Include(p => p.PostImages.OrderBy(pi => pi.DisplayOrder))
            .Include(p => p.PostTags)
            .ToListAsync(ct);
    }

    public async Task IncrementViewCountAsync(Guid postId, CancellationToken ct = default)
    {
        await context.Posts
            .Where(p => p.Id == postId)
            .ExecuteUpdateAsync(
                s => s.SetProperty(p => p.ViewCount, p => p.ViewCount + 1),
                ct);
    }

    public async Task<(List<Post> Posts, int Total)> SearchAsync(
        string query,
        int page = 1,
        int pageSize = 10,
        CancellationToken ct = default)
    {
        // ── Validate query ──────────────────────────
        if (string.IsNullOrWhiteSpace(query))
            return (new List<Post>(), 0);

        // ── Prepare search query ────────────────────
        var searchQuery = query.Trim().ToLower();

        // ── Build base query ────────────────────────
        var baseQuery = context.Posts
            .AsNoTracking()
            .Where(p => p.Status == "active" && 
                   (EF.Functions.ToTsVector("english", p.Title + " " + p.Content)
                    .Matches(EF.Functions.PlainToTsQuery("english", searchQuery))))
            .OrderByDescending(p => EF.Functions.ToTsVector("english", p.Title + " " + p.Content)
                .Rank(EF.Functions.PlainToTsQuery("english", searchQuery)))
            .ThenByDescending(p => p.CreatedAt);

        // ── Get total count ─────────────────────────
        var total = await baseQuery.CountAsync(ct);

        // ── Get paginated results ───────────────────
        var posts = await baseQuery
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Include(p => p.Author)
            .Include(p => p.Subject)
            .Include(p => p.PostImages.OrderBy(pi => pi.DisplayOrder))
            .Include(p => p.PostTags)
            .ToListAsync(ct);

        return (posts, total);
    }
}
