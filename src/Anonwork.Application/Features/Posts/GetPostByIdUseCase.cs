using Anonwork.Application.Features.Posts.DTOs;
using Anonwork.Application.Interfaces;
using Anonwork.Domain.Common.Exceptions;
using Post = Anonwork.Domain.Entities.Post;

namespace Anonwork.Application.Features.Posts;

/// <summary>
/// Use case for getting a post by id
/// </summary>
public class GetPostByIdUseCase(IPostRepository postRepo)
{
    public async Task<PostResponseDto> ExecuteAsync(Guid postId, CancellationToken ct = default)
    {
        // ── Validation ──────────────────────────────
        if (postId == Guid.Empty)
            throw new ArgumentException("Post id is required.");

        // ── Get post ────────────────────────────────
        var post = await postRepo.GetByIdWithDetailsAsync(postId, ct);

        if (post is null)
            throw new NotFoundException(nameof(Post), postId);

        // ── Increment view count ────────────────────
        await postRepo.IncrementViewCountAsync(postId, ct);

        // ── Return response ─────────────────────────
        return MapToResponse(post);
    }

    private static PostResponseDto MapToResponse(Post post)
    {
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
            ImageUrls: post.PostImages
                .OrderBy(pi => pi.DisplayOrder)
                .Select(pi => pi.ImageUrl)
                .ToList(),
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
