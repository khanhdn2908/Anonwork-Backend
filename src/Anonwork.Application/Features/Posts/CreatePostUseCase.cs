using Anonwork.Application.Interfaces;
using Anonwork.Application.Features.Posts.DTOs.Request;
using Anonwork.Application.Features.Posts.DTOs.Response;
using Anonwork.Domain.Entities;
using Anonwork.Domain.Enums;

namespace Anonwork.Application.Features.Posts;

/// <summary>
/// Use case for creating a new post
/// </summary>
public class CreatePostUseCase(IUnitOfWork unitOfWork, IPostMediaService postMediaService, IAppDbContext dbContext, IPlanAccessService planAccessService)
{
    private readonly IGenericRepository<Post> _postRepo = unitOfWork.GetRepository<Post>();
    private readonly IPostMediaService _postMediaService = postMediaService;
    private readonly IAppDbContext _dbContext = dbContext;
    private readonly IPlanAccessService _planAccessService = planAccessService;

    public async Task<PostResponseDto> ExecuteAsync(CreatePostRequest req, CancellationToken ct = default)
    {
        await _planAccessService.EnsureCanCreatePostAsync(req.AuthorId, req.Images, req.File, ct);

        if (string.IsNullOrWhiteSpace(req.Title))
            throw new ArgumentException("Title is required.");

        if (string.IsNullOrWhiteSpace(req.Content))
            throw new ArgumentException("Content is required.");

        var post = new Post
        {
            Id = Guid.NewGuid(),
            AuthorId = req.AuthorId,
            SubjectId = req.SubjectId,
            Title = req.Title.Trim(),
            Content = req.Content.Trim(),
            IsAnonymous = req.IsAnonymous,
            Status = PostStatus.Published,
            Upvotes = 0,
            CommentsCount = 0,
            ViewCount = 0,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        if (req.Tags is not null && req.Tags.Count > 0)
        {
            post.PostTags = req.Tags
                .Take(5)
                .Select(tag => new PostTag
                {
                    PostId = post.Id,
                    Tag = tag.Trim().ToLower()
                })
                .ToList();
        }

        post.PostMediaItems = await _postMediaService.BuildPostMediaAsync(
            post.Id,
            req.Images,
            req.File,
            ct);

        await using var transaction = await _dbContext.BeginTransactionAsync(ct);
        try
        {
            await _postRepo.AddAsync(post, ct);
            await unitOfWork.SaveChangesAsync(ct);
            await transaction.CommitAsync(ct);

            return MapToResponse(post);
        }
        catch
        {
            await transaction.RollbackAsync(ct);
            throw;
        }
    }

    private static PostResponseDto MapToResponse(Post post)
    {
        var author = post.Author;
        var isAnon = post.IsAnonymous || author?.IsAnonDefault == true;
        var media = post.PostMediaItems
            .OrderBy(pm => pm.DisplayOrder)
            .Select(pm => new PostMediaResponseDto(
                pm.Id,
                pm.FileKey,
                pm.FileKey,
                pm.ContentType,
                pm.DisplayOrder,
                pm.FileSize,
                pm.OriginalFileName,
                pm.MediaType.ToString()))
            .ToList();

        var authorImageUrl = isAnon
            ? (!string.IsNullOrWhiteSpace(author?.AnonImage?.FileKey)
                ? author!.AnonImage!.FileKey
                : "avatars/null.jpg")
            : (string.IsNullOrWhiteSpace(author?.AvatarKey)
                ? "avatars/null.jpg"
                : author!.AvatarKey);

        return new PostResponseDto(
            Id: post.Id,
            Title: post.Title,
            Content: post.Content,
            AuthorId: post.AuthorId,
            AuthorUsername: isAnon ? author?.AnonAlias : author?.Username,
            IsAnonymous: isAnon,
            AuthorAvatarUrl: authorImageUrl,
            SubjectId: post.SubjectId,
            SubjectName: post.Subject?.Name,
            Media: media,
            Tags: post.PostTags
                .Select(pt => pt.Tag)
                .ToList(),
            Upvotes: post.Upvotes,
            CommentsCount: post.CommentsCount,
            ViewCount: post.ViewCount,
            AverageRating: post.AverageRating,
            RatingsCount: post.RatingsCount,
            QualityScore: post.QualityScore,
            MyStars: null,
            Status: post.Status.ToString(),
            CreatedAt: post.CreatedAt,
            UpdatedAt: post.UpdatedAt,
            IsUpvotedByMe: false
        );
    }
}
