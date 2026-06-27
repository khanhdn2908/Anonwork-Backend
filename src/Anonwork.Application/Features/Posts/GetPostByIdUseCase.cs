using Anonwork.Application.Common.Exceptions;
using Anonwork.Application.Features.Posts.DTOs.Response;
using Anonwork.Application.Features.Posts.Helpers;
using Anonwork.Application.Interfaces;
using Anonwork.Domain.Entities;
using Anonwork.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Anonwork.Application.Features.Posts;

/// <summary>
/// Use case for getting a post by id
/// </summary>
public class GetPostByIdUseCase(IUnitOfWork unitOfWork, IR2Service r2Service)
{
    private readonly IGenericRepository<Post> _postRepo = unitOfWork.GetRepository<Post>();
    private readonly IGenericRepository<Vote> _voteRepo = unitOfWork.GetRepository<Vote>();
    private readonly IR2Service _r2Service = r2Service;

    public async Task<PostResponseDto> ExecuteAsync(Guid postId, bool hasPermission, Guid? currentUserId = null, CancellationToken ct = default)
    {
        if (postId == Guid.Empty)
            throw new ArgumentException("Post id is required.");

        var post = await _postRepo.GetQueryableNoTracking()
            .Include(p => p.Author)
            .ThenInclude(a => a.AnonImage)
            .Include(p => p.Subject)
            .Include(p => p.PostMediaItems)
            .Include(p => p.PostTags)
            .FirstOrDefaultAsync(p => p.Id == postId, ct);

        if (post is null)
            throw new NotFoundException(nameof(Post), postId);

        if (!hasPermission && post.Status != PostStatus.Published)
            throw new NotFoundException(nameof(Post), postId);

        post.ViewCount += 1;
        post.UpdatedAt = DateTime.UtcNow;
        await _postRepo.UpdateAsync(post, ct);
        await unitOfWork.SaveChangesAsync(ct);

        var isUpvotedByMe = currentUserId.HasValue && await _voteRepo.GetQueryableNoTracking().AnyAsync(
            v => v.UserId == currentUserId.Value && v.TargetId == postId && v.TargetType == "post" && v.VoteType == "up",
            ct);

        return PostVoteProjectionHelper.MapToResponse(post, isUpvotedByMe, _r2Service);
    }
}
