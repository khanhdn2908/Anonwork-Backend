using Anonwork.Application.Common.Exceptions;
using Anonwork.Application.Interfaces;
using Anonwork.Domain.Common.Exceptions;
using Post = Anonwork.Domain.Entities.Post;

namespace Anonwork.Application.Features.Posts;

/// <summary>
/// Use case for deleting a post
/// </summary>
public class DeletePostUseCase(IPostRepository postRepo, ICloudinaryService cloudinaryService)
{
    public async Task ExecuteAsync(Guid postId, Guid userId, CancellationToken ct = default)
    {
        // ── Validation ──────────────────────────────
        if (postId == Guid.Empty)
            throw new ArgumentException("Post id is required.");

        // ── Get post ────────────────────────────────
        var post = await postRepo.GetByIdWithDetailsAsync(postId, ct);
        if (post is null)
            throw new NotFoundException(nameof(Post), postId);

        // ── Authorization ──────────────────────────
        if (post.AuthorId != userId)
            throw new UnauthorizedException("You can only delete your own posts.");

        // ── Delete images from Cloudinary ──────────
        if (post.PostImages.Count > 0)
        {
            try
            {
                var imageUrls = post.PostImages.Select(pi => pi.ImageUrl).ToList();
                await cloudinaryService.DeleteImagesAsync(imageUrls, ct);
            }
            catch (Exception ex)
            {
                // Log error but continue with post deletion
                Console.WriteLine($"Failed to delete images from Cloudinary: {ex.Message}");
            }
        }

        // ── Soft delete post ────────────────────────
        await postRepo.DeleteAsync(postId, ct);
    }
}
