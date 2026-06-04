using Anonwork.Application.Features.Posts.DTOs;
using Anonwork.Application.Interfaces;
using Anonwork.Domain.Common.Exceptions;
using Microsoft.EntityFrameworkCore;
using Post = Anonwork.Domain.Entities.Post;

namespace Anonwork.Application.Features.Posts;

/// <summary>
/// Use case for getting a post by id
/// </summary>
public class GetPostByIdUseCase(IUnitOfWork unitOfWork)
{
    private readonly IGenericRepository<Post> _postRepo = unitOfWork.GetRepository<Post>();

    public async Task<PostResponseDto> ExecuteAsync(Guid postId, CancellationToken ct = default)
    {
        // ── Validation ──────────────────────────────
        if (postId == Guid.Empty)
            throw new ArgumentException("Post id is required.");

        // ── Get post ────────────────────────────────
        var post = await _postRepo.GetQueryableNoTracking()
            .Include(p => p.Author)
            .Include(p => p.Subject)
            .Include(p => p.PostImages)
            .Include(p => p.PostTags)
            .FirstOrDefaultAsync(p => p.Id == postId, ct);

        if (post is null)
            throw new NotFoundException(nameof(Post), postId);

        // ── Increment view count ────────────────────
        post.ViewCount += 1;
        post.UpdatedAt = DateTime.UtcNow;
        await _postRepo.UpdateAsync(post, ct);
        await unitOfWork.SaveChangesAsync(ct);

        // ── Return response ─────────────────────────
        return MapToResponse(post);
    }

    private static PostResponseDto MapToResponse(Post post)
    {
        var imageUrls = post.PostImages
            .OrderBy(pi => pi.DisplayOrder)
            .Select(pi => pi.ImageUrl)
            .ToList();

        return new PostResponseDto(
            Id: post.Id,
            Title: post.Title,
            Content: post.Content,
            AuthorId: post.AuthorId,
            AuthorUsername: post.Author?.Username,
            AuthorAnonAlias: post.Author?.AnonAlias,
            IsAnonymous: post.IsAnonymous,
            SubjectId: post.SubjectId,
            SubjectName: post.Subject?.Name,
            ImageUrls: imageUrls,
            RemainingImagesCount: 0,
            Tags: post.PostTags
                .Select(pt => pt.Tag)
                .ToList(),
            Upvotes: post.Upvotes,
            CommentsCount: post.CommentsCount,
            ViewCount: post.ViewCount,
            Status: post.Status,
            CreatedAt: post.CreatedAt,
            UpdatedAt: post.UpdatedAt
        );
    }
}
