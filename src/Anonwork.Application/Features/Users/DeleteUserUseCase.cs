using Anonwork.Application.Common.Exceptions;
using Anonwork.Application.Interfaces;
using Anonwork.Domain.Entities;
using Anonwork.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Anonwork.Application.Features.Users;

public class DeleteUserUseCase(IUnitOfWork unitOfWork)
{
    private readonly IGenericRepository<User> _userRepo = unitOfWork.GetRepository<User>();
    private readonly IGenericRepository<Post> _postRepo = unitOfWork.GetRepository<Post>();
    private readonly IGenericRepository<Comment> _commentRepo = unitOfWork.GetRepository<Comment>();
    private readonly IGenericRepository<Vote> _voteRepo = unitOfWork.GetRepository<Vote>();
    private readonly IGenericRepository<Bookmark> _bookmarkRepo = unitOfWork.GetRepository<Bookmark>();
    private readonly IGenericRepository<Follow> _followRepo = unitOfWork.GetRepository<Follow>();
    private readonly IGenericRepository<UserSubscription> _userSubscriptionRepo = unitOfWork.GetRepository<UserSubscription>();

    public async Task ExecuteAsync(Guid userId, CancellationToken ct = default)
    {
        var user = await _userRepo.GetByIdWithTrackingAsync(userId, ct)
            ?? throw new NotFoundException("User not found.");

        user.Status = UserStatus.Deleted;
        user.UpdatedAt = DateTime.UtcNow;

        var posts = await _postRepo.GetQueryableNoTracking()
            .Where(p => p.AuthorId == userId && p.Status != PostStatus.Deleted)
            .ToListAsync(ct);
        foreach (var post in posts)
        {
            post.Status = PostStatus.Deleted;
            post.UpdatedAt = DateTime.UtcNow;
        }
        if (posts.Count > 0)
            await _postRepo.UpdateRangeAsync(posts, ct);

        var comments = await _commentRepo.GetQueryableNoTracking()
            .Where(c => c.AuthorId == userId && c.IsActive)
            .ToListAsync(ct);
        foreach (var comment in comments)
        {
            comment.IsActive = false;
            comment.UpdatedAt = DateTime.UtcNow;
        }
        if (comments.Count > 0)
            await _commentRepo.UpdateRangeAsync(comments, ct);

        var votes = await _voteRepo.GetQueryableNoTracking()
            .Where(v => v.UserId == userId)
            .ToListAsync(ct);
        if (votes.Count > 0)
            await _voteRepo.DeleteRangeAsync(votes, ct);

        var bookmarks = await _bookmarkRepo.GetQueryableNoTracking()
            .Where(b => b.UserId == userId)
            .ToListAsync(ct);
        if (bookmarks.Count > 0)
            await _bookmarkRepo.DeleteRangeAsync(bookmarks, ct);

        var follows = await _followRepo.GetQueryableNoTracking()
            .Where(f => f.FollowerId == userId || f.FollowingId == userId)
            .ToListAsync(ct);
        if (follows.Count > 0)
            await _followRepo.DeleteRangeAsync(follows, ct);

        var subscriptions = await _userSubscriptionRepo.GetQueryableNoTracking()
            .Where(s => s.UserId == userId && s.Status == SubscriptionStatus.Active)
            .ToListAsync(ct);
        foreach (var subscription in subscriptions)
        {
            subscription.Status = SubscriptionStatus.Cancelled;
        }
        if (subscriptions.Count > 0)
            await _userSubscriptionRepo.UpdateRangeAsync(subscriptions, ct);

        await unitOfWork.SaveChangesAsync(ct);
    }
}
