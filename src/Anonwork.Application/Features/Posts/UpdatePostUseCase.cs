using Anonwork.Application.Common.Exceptions;
using Anonwork.Application.Features.Posts.DTOs.Request;
using Anonwork.Application.Features.Posts.DTOs.Response;
using Anonwork.Application.Features.Posts.Helpers;
using Anonwork.Application.Interfaces;
using Anonwork.Domain.Entities;
using Anonwork.Domain.Enums;
using Microsoft.AspNetCore.Http;

namespace Anonwork.Application.Features.Posts;

public class UpdatePostUseCase(IUnitOfWork unitOfWork, IPostMediaService postMediaService, IAppDbContext dbContext)
{
    private readonly IGenericRepository<Post> _postRepo = unitOfWork.GetRepository<Post>();
    private readonly IPostMediaService _postMediaService = postMediaService;
    private readonly IAppDbContext _dbContext = dbContext;

    public async Task<PostResponseDto> ExecuteAsync(UpdatePostRequest req, CancellationToken ct = default)
    {
        if (req.PostId == Guid.Empty)
            throw new ArgumentException("Post id is required.");

        var post = await _postRepo.FindSingleWithTrackingAsync(p => p.Id == req.PostId, ct);
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
            post.PostTags.Clear();
            foreach (var tag in req.Tags.Take(5))
            {
                post.PostTags.Add(new PostTag
                {
                    PostId = post.Id,
                    Tag = tag.Trim().ToLower()
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

    private static PostResponseDto MapToResponse(Post post)
    {
        var isAnon = post.IsAnonymous && post.Author.IsAnonDefault;
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

        return new PostResponseDto(
            Id: post.Id,
            Title: post.Title,
            Content: post.Content,
            AuthorId: post.AuthorId,
            AuthorUsername: isAnon ? null : post.Author?.Username,
            AuthorAnonAlias: isAnon ? post.Author?.AnonAlias : null,
            IsAnonymous: isAnon,
            SubjectId: post.SubjectId,
            SubjectName: post.Subject?.Name,
            Media: media,
            Tags: post.PostTags.Select(pt => pt.Tag).ToList(),
            Upvotes: post.Upvotes,
            CommentsCount: post.CommentsCount,
            ViewCount: post.ViewCount,
            Status: post.Status.ToString(),
            CreatedAt: post.CreatedAt,
            UpdatedAt: post.UpdatedAt,
            IsUpvotedByMe: false
        );
    }
}
