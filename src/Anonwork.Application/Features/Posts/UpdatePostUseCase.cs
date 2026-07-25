using Anonwork.Application.Common.Exceptions;
using Anonwork.Application.Features.Posts.DTOs.Request;
using Anonwork.Application.Features.Posts.DTOs.Response;
using Anonwork.Application.Features.Posts.Helpers;
using Anonwork.Application.Interfaces;
using Anonwork.Domain.Entities;
using Anonwork.Domain.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Anonwork.Application.Features.Posts;

public class UpdatePostUseCase(IUnitOfWork unitOfWork, IPostMediaService postMediaService, IAppDbContext dbContext, IR2Service r2Service)
{
    private readonly IGenericRepository<Post> _postRepo = unitOfWork.GetRepository<Post>();
    private readonly IPostMediaService _postMediaService = postMediaService;
    private readonly IAppDbContext _dbContext = dbContext;
    private readonly IR2Service _r2Service = r2Service;

    public async Task<PostResponseDto> ExecuteAsync(UpdatePostRequest req, CancellationToken ct = default)
    {
        if (req.PostId == Guid.Empty)
            throw new ArgumentException("Post id is required.");

        var post = await _dbContext.Posts
            .Include(p => p.PostTags)
            .Include(p => p.PostMediaItems)
            .Include(p => p.Author)
                .ThenInclude(a => a.AnonImage)
            .Include(p => p.Subject)
            .FirstOrDefaultAsync(p => p.Id == req.PostId, ct);

        if (post is null)
            throw new NotFoundException(nameof(Post), req.PostId);

        if (post.AuthorId != req.AuthorId)
            throw new UnauthorizedException("You can only update your own posts.");

        if (!string.IsNullOrWhiteSpace(req.Title))
            post.Title = req.Title.Trim();

        if (!string.IsNullOrWhiteSpace(req.Content))
            post.Content = req.Content.Trim();

        if (req.Tags is not null)
        {
            post.PostTags ??= new List<PostTag>();
            post.PostTags.Clear();
            var validTags = req.Tags
                .Where(t => !string.IsNullOrWhiteSpace(t))
                .Select(t => t.Trim().ToLower())
                .Distinct()
                .Take(5);

            foreach (var tag in validTags)
            {
                post.PostTags.Add(new PostTag
                {
                    PostId = post.Id,
                    Tag = tag
                });
            }
        }

        var mediaToRemove = new List<PostMedia>();
        if (req.RemoveMediaId is not null && req.RemoveMediaId.Count > 0)
        {
            mediaToRemove = post.PostMediaItems
                .Where(pm => req.RemoveMediaId.Contains(pm.Id))
                .ToList();
        }

        var remainingMedia = post.PostMediaItems
            .Where(pm => !mediaToRemove.Contains(pm))
            .ToList();

        if (req.ReplaceMedia)
            remainingMedia.Clear();

        var finalMedia = await _postMediaService.AppendPostMediaAsync(
            post.Id,
            remainingMedia,
            req.Images,
            req.Files,
            ct);

        post.PostMediaItems = finalMedia;
        post.UpdatedAt = DateTime.UtcNow;

        await using var transaction = await _dbContext.BeginTransactionAsync(ct);
        try
        {
            await _postRepo.UpdateAsync(post, ct);
            await unitOfWork.SaveChangesAsync(ct);
            await transaction.CommitAsync(ct);
        }
        catch
        {
            await transaction.RollbackAsync(ct);
            await _postMediaService.RemoveMediaFilesAsync(finalMedia.Except(remainingMedia), ct);
            throw;
        }

        await _postMediaService.RemoveMediaFilesAsync(mediaToRemove, ct);

        return MapToResponse(post);
    }

    private PostResponseDto MapToResponse(Post post)
    {
        var author = post.Author;
        var isAnon = post.IsAnonymous || author?.IsAnonDefault == true;
        var media = (post.PostMediaItems ?? new List<PostMedia>())
            .OrderBy(pm => pm.DisplayOrder)
            .Select(pm => new PostMediaResponseDto(
                pm.Id,
                pm.FileKey,
                _r2Service.GetPublicUrl(pm.FileKey),
                pm.ContentType,
                pm.DisplayOrder,
                pm.FileSize,
                pm.OriginalFileName,
                pm.MediaType.ToString()))
            .ToList();

        var authorImageUrl = isAnon
            ? (!string.IsNullOrWhiteSpace(author?.AnonImage?.FileKey)
                ? _r2Service.GetPublicUrl(author!.AnonImage!.FileKey)
                : _r2Service.GetPublicUrl("avatars/null.jpg"))
            : (string.IsNullOrWhiteSpace(author?.AvatarKey)
                ? _r2Service.GetPublicUrl("avatars/null.jpg")
                : _r2Service.GetPublicUrl(author!.AvatarKey));

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
            Tags: (post.PostTags ?? new List<PostTag>()).Select(pt => pt.Tag).ToList(),
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
