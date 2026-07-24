using Anonwork.Application.Features.Posts.DTOs.Response;
using Anonwork.Application.Interfaces;
using Anonwork.Domain.Entities;
using Anonwork.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Anonwork.Application.Features.Posts;

/// <summary>
/// Use case for getting posts with pagination and search.
/// </summary>
public class GetPostsUseCase(IUnitOfWork unitOfWork, IR2Service r2Service)
{
    private readonly IGenericRepository<Post> _postRepo = unitOfWork.GetRepository<Post>();
    private readonly IGenericRepository<Vote> _voteRepo = unitOfWork.GetRepository<Vote>();
    private readonly IR2Service _r2Service = r2Service;

    public async Task<PostListResponseDto> ExecuteAsync(
        bool hasPermission,
        int page = 1,
        int pageSize = 10,
        string? searchQuery = null,
        Guid? currentUserId = null,
        CancellationToken ct = default)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 10;
        if (pageSize > 100) pageSize = 100;

        var query = _postRepo.GetQueryableNoTracking()
            .Include(p => p.Author)
            .ThenInclude(a => a.AnonImage)
            .Include(p => p.Subject)
            .Include(p => p.PostMediaItems)
            .Include(p => p.PostTags)
            .Where(p => hasPermission || p.Status == PostStatus.Published);

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

        var totalPages = (int)Math.Ceiling((double)total / pageSize);

        var postIds = posts.Select(p => p.Id).ToList();
        var upvotedSet = currentUserId.HasValue && postIds.Count > 0
            ? (await _voteRepo.GetQueryableNoTracking()
                .Where(v => v.UserId == currentUserId.Value
                    && v.TargetType == "post"
                    && postIds.Contains(v.TargetId)
                    && v.VoteType == "up")
                .Select(v => v.TargetId)
                .ToListAsync(ct)).ToHashSet()
            : new HashSet<Guid>();

        var ratingRepo = unitOfWork.GetRepository<PostRating>();
        var myRatingsDict = currentUserId.HasValue && postIds.Count > 0
            ? await ratingRepo.GetQueryableNoTracking()
                .Where(r => r.UserId == currentUserId.Value && postIds.Contains(r.PostId))
                .ToDictionaryAsync(r => r.PostId, r => r.Stars, ct)
            : new Dictionary<Guid, int>();

        var postDtos = posts.Select(p =>
        {
            var isAnon = p.IsAnonymous || p.Author.IsAnonDefault;
            var media = p.PostMediaItems
                .OrderBy(i => i.DisplayOrder)
                .Select(i => new PostMediaResponseDto(
                    i.Id,
                    i.FileKey,
                    _r2Service.GetPublicUrl(i.FileKey),
                    i.ContentType,
                    i.DisplayOrder,
                    i.FileSize,
                    i.OriginalFileName,
                    i.MediaType.ToString()))
                .ToList();

            var tags = p.PostTags
                .Select(t => t.Tag)
                .ToList();

            var authorImageUrl = isAnon
                ? (!string.IsNullOrWhiteSpace(p.Author.AnonImage?.FileKey)
                    ? _r2Service.GetPublicUrl(p.Author.AnonImage.FileKey)
                    : _r2Service.GetPublicUrl("avatars/null.jpg"))
                : (string.IsNullOrWhiteSpace(p.Author.AvatarKey)
                    ? _r2Service.GetPublicUrl("avatars/null.jpg")
                    : _r2Service.GetPublicUrl(p.Author.AvatarKey));

            return new PostResponseDto(
                p.Id,
                p.Title,
                p.Content,
                p.AuthorId,
                isAnon ? p.Author.AnonAlias : p.Author.Username,
                isAnon,
                authorImageUrl,
                p.SubjectId,
                p.Subject?.Name,
                media,
                tags,
                p.Upvotes,
                p.CommentsCount,
                p.ViewCount,
                p.AverageRating,
                p.RatingsCount,
                p.QualityScore,
                myRatingsDict.TryGetValue(p.Id, out var stars) ? stars : null,
                p.Status.ToString(),
                p.CreatedAt,
                p.UpdatedAt,
                upvotedSet.Contains(p.Id));
        }).ToList();

        return new PostListResponseDto(postDtos, total, page, pageSize, totalPages);
    }
}
