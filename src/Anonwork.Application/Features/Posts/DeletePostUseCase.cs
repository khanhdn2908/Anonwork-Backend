using Anonwork.Application.Common.Exceptions;
using Anonwork.Application.Interfaces;
using Anonwork.Domain.Common.Exceptions;
using Anonwork.Domain.Entities;
using Post = Anonwork.Domain.Entities.Post;

namespace Anonwork.Application.Features.Posts;

/// <summary>
/// Use case for deleting a post
/// </summary>
public class DeletePostUseCase(IUnitOfWork unitOfWork, ICloudinaryService cloudinaryService)
{
    private readonly IGenericRepository<Post> _postRepo = unitOfWork.GetRepository<Post>();
    private readonly IGenericRepository<User> _userRepo = unitOfWork.GetRepository<User>();

    public async Task ExecuteAsync(Guid postId, Guid userId, CancellationToken ct = default)
    {
        // ── Validation ──────────────────────────────
        if (postId == Guid.Empty)
            throw new ArgumentException("Post id is required.");

        // ── Get post ────────────────────────────────
        var post = await _postRepo.FindSingleWithTrackingAsync(p => p.Id == postId, ct);
        if (post is null)
            throw new NotFoundException(nameof(Post), postId);

        // ── Get user to check role ──────────────────
        var user = await _userRepo.GetByIdAsync(userId, ct);
        if (user is null)
            throw new UnauthorizedException("User not found.");

        // ── Authorization ──────────────────────────
        // Allow if: author OR admin OR moderator
        var isAuthor = post.AuthorId == userId;
        //var isAdmin = user.Role == "admin";
        //var isModerator = user.Role == "moderator";

        //if (!isAuthor && !isAdmin && !isModerator)
        //    throw new UnauthorizedException("You can only delete your own posts. Only admins and moderators can delete other posts.");

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
        await _postRepo.DeleteAsync(postId, ct);
        await unitOfWork.SaveChangesAsync(ct);
    }
}

