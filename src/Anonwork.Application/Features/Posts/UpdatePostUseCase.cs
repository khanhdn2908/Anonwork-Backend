using Anonwork.Application.Common.Exceptions;
using Anonwork.Application.Features.Posts.DTOs.Request;
using Anonwork.Application.Features.Posts.DTOs.Response;
using Anonwork.Application.Interfaces;
using Anonwork.Domain.Common.Exceptions;
using Anonwork.Domain.Entities;
using Post = Anonwork.Domain.Entities.Post;

namespace Anonwork.Application.Features.Posts;

/// <summary>
/// Use case for updating a post
/// </summary>
public class UpdatePostUseCase(IUnitOfWork unitOfWork, ICloudinaryService cloudinaryService)
{
    private readonly IGenericRepository<Post> _postRepo = unitOfWork.GetRepository<Post>();

    public async Task<PostResponseDto> ExecuteAsync(UpdatePostRequest req, CancellationToken ct = default)
    {
        // ── Validation ──────────────────────────────
        if (req.PostId == Guid.Empty)
            throw new ArgumentException("Post id is required.");

        // ── Get post ────────────────────────────────
        var post = await _postRepo.FindSingleWithTrackingAsync(p => p.Id == req.PostId, ct);
        if (post is null)
            throw new NotFoundException(nameof(Post), req.PostId);

        // ── Authorization ──────────────────────────
        if (post.AuthorId != req.AuthorId)
            throw new UnauthorizedException("You can only update your own posts.");

        // ── Update fields ──────────────────────────
        if (!string.IsNullOrWhiteSpace(req.Title))
            post.Title = req.Title.Trim();

        if (!string.IsNullOrWhiteSpace(req.Content))
            post.Content = req.Content.Trim();

        // ── Update tags ────────────────────────────
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

        // ── Remove images ──────────────────────────
        if (req.RemoveImageUrls is not null && req.RemoveImageUrls.Count > 0)
        {
            var imagesToRemove = post.PostImages
                .Where(pi => req.RemoveImageUrls.Contains(pi.ImageUrl))
                .ToList();

            foreach (var image in imagesToRemove)
            {
                post.PostImages.Remove(image);
            }
        }

        // ── Add new images ────────────────────────
        if (req.NewImageUrls is not null && req.NewImageUrls.Count > 0)
        {
            var currentImageCount = post.PostImages.Count;
            var maxNewImages = 5 - currentImageCount;

            if (maxNewImages > 0)
            {
                var startOrder = post.PostImages.Count > 0
                    ? post.PostImages.Max(pi => pi.DisplayOrder) + 1
                    : 0;

                foreach (var (url, index) in req.NewImageUrls.Take(maxNewImages).Select((u, i) => (u, i)))
                {
                    post.PostImages.Add(new PostImage
                    {
                        Id = Guid.NewGuid(),
                        PostId = post.Id,
                        ImageUrl = url,
                        DisplayOrder = startOrder + index,
                        CreatedAt = DateTime.UtcNow
                    });
                }
            }
        }

        // ── Update timestamp ───────────────────────
        post.UpdatedAt = DateTime.UtcNow;

        // ── Save to database ───────────────────────
        await _postRepo.UpdateAsync(post, ct);
        await unitOfWork.SaveChangesAsync(ct);

        // ── Return response ────────────────────────
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
            Status: post.Status,
            CreatedAt: post.CreatedAt,
            UpdatedAt: post.UpdatedAt,
            false
        );
    }
}
