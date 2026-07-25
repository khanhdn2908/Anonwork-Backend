using Anonwork.Application.Common.Exceptions;
using Anonwork.Application.Interfaces;
using Anonwork.Domain.Entities;
using Anonwork.Domain.Enums;

namespace Anonwork.Application.Features.Posts;

/// <summary>
/// Use case for deleting a post
/// </summary>
public class DeletePostUseCase(IUnitOfWork unitOfWork, IPostMediaService postMediaService, IAppDbContext dbContext, IActivityLogService activityLogService)
{
    private readonly IGenericRepository<Post> _postRepo = unitOfWork.GetRepository<Post>();
    private readonly IGenericRepository<User> _userRepo = unitOfWork.GetRepository<User>();
    private readonly IPostMediaService _postMediaService = postMediaService;
    private readonly IAppDbContext _dbContext = dbContext;
    private readonly IActivityLogService _activityLogService = activityLogService;

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

        var mediaItems = post.PostMediaItems.ToList();

        await using var transaction = await _dbContext.BeginTransactionAsync(ct);
        try
        {
            await _postMediaService.RemoveMediaFilesAsync(mediaItems, ct);

            post.Status = PostStatus.Deleted;
            post.UpdatedAt = DateTime.UtcNow;
            await unitOfWork.SaveChangesAsync(ct);

            await transaction.CommitAsync(ct);

            _ = _activityLogService.LogAsync(
                userId,
                "DELETE_POST",
                "Post",
                $"Xóa bài viết '{post.Title}'",
                "post",
                post.Id.ToString(),
                ct: ct);
        }
        catch
        {
            await transaction.RollbackAsync(ct);
            throw;
        }
    }
}

