using Anonwork.Application.Interfaces;
using Anonwork.Domain.Entities;
using Anonwork.Domain.Common.Exceptions;
using Post = Anonwork.Domain.Entities.Post;
using Anonwork.Application.Features.Posts.DTOs.Request;
using Anonwork.Application.Features.Posts.DTOs.Response;
using Anonwork.Domain.Enums;

namespace Anonwork.Application.Features.Posts;

/// <summary>
/// Use case for creating a new post
/// </summary>
public class CreatePostUseCase(IUnitOfWork unitOfWork)
{
    private readonly IGenericRepository<Post> _postRepo = unitOfWork.GetRepository<Post>();

    public async Task<PostResponseDto> ExecuteAsync(CreatePostRequest req, CancellationToken ct = default)
    {
        // ── Validation ──────────────────────────────
        if (string.IsNullOrWhiteSpace(req.Title))
            throw new ArgumentException("Title is required.");

        if (string.IsNullOrWhiteSpace(req.Content))
            throw new ArgumentException("Content is required.");

        // ── Create post ─────────────────────────────
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

        // ── Add tags if provided ────────────────────
        if (req.Tags is not null && req.Tags.Count > 0)
        {
            post.PostTags = req.Tags
                .Take(5) // Max 5 tags
                .Select(tag => new PostTag
                {
                    PostId = post.Id,
                    Tag = tag.Trim().ToLower()
                })
                .ToList();
        }

        // ── Add images if provided ──────────────────
        if (req.ImageUrls is not null && req.ImageUrls.Count > 0)
        {
            post.PostImages = req.ImageUrls
                .Take(5) // Max 5 images
                .Select((url, index) => new PostImage
                {
                    Id = Guid.NewGuid(),
                    PostId = post.Id,
                    ImageUrl = url,
                    DisplayOrder = index,
                    CreatedAt = DateTime.UtcNow
                })
                .ToList();
        }

        // ── Save to database ────────────────────────
        await _postRepo.AddAsync(post, ct);
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

        var previewImageUrls = imageUrls.Take(2).ToList();
        var remainingImagesCount = Math.Max(0, imageUrls.Count - previewImageUrls.Count);

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
            ImageUrls: previewImageUrls,
            RemainingImagesCount: remainingImagesCount,
            Tags: post.PostTags
                .Select(pt => pt.Tag)
                .ToList(),
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
