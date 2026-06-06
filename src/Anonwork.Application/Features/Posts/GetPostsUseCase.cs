using Anonwork.Application.Features.Posts.DTOs.Response;
using Anonwork.Application.Interfaces;
using Anonwork.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Anonwork.Application.Features.Posts;

/// <summary>
/// Use case for getting posts with pagination and search
/// </summary>
public class GetPostsUseCase(IUnitOfWork unitOfWork)
{
    private readonly IGenericRepository<Post> _postRepo = unitOfWork.GetRepository<Post>();
    private readonly IGenericRepository<Anonwork.Domain.Entities.Vote> _voteRepo = unitOfWork.GetRepository<Anonwork.Domain.Entities.Vote>();

    public async Task<PostListResponseDto> ExecuteAsync(
        int page = 1,
        int pageSize = 10,
        string? searchQuery = null,
        Guid? currentUserId = null,
        CancellationToken ct = default)
    {
        // ── Validation ──────────────────────────────
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 10;
        if (pageSize > 100) pageSize = 100; // Max 100 per page

        // ── Get posts ───────────────────────────────
        IQueryable<Post> query = _postRepo.GetQueryableNoTracking()
            .Include(p => p.Author)
            .Include(p => p.Subject)
            .Include(p => p.PostImages)
            .Include(p => p.PostTags);

        if (!string.IsNullOrWhiteSpace(searchQuery))
        {
            var keyword = searchQuery.Trim();
            query = query.Where(p =>
                p.Title.Contains(keyword) ||
                p.Content.Contains(keyword));
        }

        var total = await query.CountAsync(ct);
        var posts = await query
            .OrderByDescending(p => p.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        // ── Calculate pagination ────────────────────
        var totalPages = (int)Math.Ceiling((double)total / pageSize);

        var postIds = posts.Select(p => p.Id).ToList();
        var upvotedSet = currentUserId.HasValue
            ? (await _voteRepo.GetQueryableNoTracking()
                .Where(v => v.UserId == currentUserId.Value && v.TargetType == "post" && postIds.Contains(v.TargetId) && v.VoteType == "up")
                .Select(v => v.TargetId)
                .ToListAsync(ct)).ToHashSet()
            : new HashSet<Guid>();

        // ── Return response ─────────────────────────
        var postDtos = posts.Select(p => PostVoteProjectionHelper.MapToResponse(p, upvotedSet.Contains(p.Id))).ToList();
        return new PostListResponseDto(postDtos, total, page, pageSize, totalPages);
    }
}
