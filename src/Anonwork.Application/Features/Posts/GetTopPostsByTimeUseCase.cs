using Anonwork.Application.Features.Posts.DTOs.Response;
using Anonwork.Application.Features.Posts.Helpers;
using Anonwork.Application.Interfaces;
using Anonwork.Domain.Entities;
using Anonwork.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Anonwork.Application.Features.Posts;

/// <summary>
/// Use case for getting top posts within a time range.
/// </summary>
public class GetTopPostsByTimeUseCase(IUnitOfWork unitOfWork, IR2Service r2Service)
{
    private readonly IGenericRepository<Post> _postRepo = unitOfWork.GetRepository<Post>();
    private readonly IGenericRepository<Vote> _voteRepo = unitOfWork.GetRepository<Vote>();
    private readonly IR2Service _r2Service = r2Service;

    public async Task<PostListResponseDto> ExecuteAsync(
        bool hasPermission,
        string timeRange,
        string sort = "hot",
        int page = 1,
        int pageSize = 10,
        Guid? currentUserId = null,
        CancellationToken ct = default)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 10;
        if (pageSize > 100) pageSize = 100;

        var now = DateTime.UtcNow;
        var from = NormalizeTimeRange(timeRange, now);
        var normalizedSort = NormalizeSort(sort);

        IQueryable<Post> query = _postRepo.GetQueryableNoTracking()
            .Include(p => p.Author)
            .Include(p => p.Subject)
            .Include(p => p.PostMediaItems)
            .Include(p => p.PostTags)
            .Include(p => p.Comments)
            .Where(p => p.CreatedAt >= from && p.CreatedAt <= now);

        if (!hasPermission)
        {
            query = query.Where(p => p.Status == PostStatus.Published);
        }

        var total = await query.CountAsync(ct);

        query = normalizedSort switch
        {
            "new" => query.OrderByDescending(p => p.CreatedAt),
            "top" => query.OrderByDescending(p => p.Upvotes)
                           .ThenByDescending(p => p.CommentsCount)
                           .ThenByDescending(p => p.ViewCount)
                           .ThenByDescending(p => p.CreatedAt),
            _ => query.OrderByDescending(p => p.Upvotes)
                      .ThenByDescending(p => p.CommentsCount)
                      .ThenByDescending(p => p.ViewCount)
                      .ThenByDescending(p => p.CreatedAt)
        };

        var posts = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        var totalPages = (int)Math.Ceiling((double)total / pageSize);

        var postIds = posts.Select(p => p.Id).ToList();
        var upvotedSet = currentUserId.HasValue
            ? (await _voteRepo.GetQueryableNoTracking()
                .Where(v => v.UserId == currentUserId.Value && v.TargetType == "post" && postIds.Contains(v.TargetId) && v.VoteType == "up")
                .Select(v => v.TargetId)
                .ToListAsync(ct)).ToHashSet()
            : new HashSet<Guid>();

        var postDtos = posts.Select(p => PostVoteProjectionHelper.MapToResponse(p, upvotedSet.Contains(p.Id), _r2Service)).ToList();

        return new PostListResponseDto(postDtos, total, page, pageSize, totalPages);
    }

    private static DateTime NormalizeTimeRange(string timeRange, DateTime now)
    {
        return timeRange.Trim().ToLowerInvariant() switch
        {
            "24h" or "day" or "1d" => now.AddHours(-24),
            "7d" or "week" or "1w" => now.AddDays(-7),
            "month" or "30d" or "1m" => now.AddDays(-30),
            _ => throw new ArgumentException("timeRange must be one of: 24h, 7d, month.")
        };
    }

    private static string NormalizeSort(string sort)
    {
        return sort.Trim().ToLowerInvariant() switch
        {
            "hot" => "hot",
            "new" => "new",
            "top" => "top",
            _ => throw new ArgumentException("sort must be one of: hot, new, top.")
        };
    }
}
