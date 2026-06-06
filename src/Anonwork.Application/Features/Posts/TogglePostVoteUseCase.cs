using Anonwork.Application.Features.Posts.DTOs.Response;
using Anonwork.Application.Interfaces;
using Anonwork.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Anonwork.Application.Features.Posts;

public class TogglePostVoteUseCase(IUnitOfWork unitOfWork)
{
    private readonly IGenericRepository<Post> _postRepository = unitOfWork.GetRepository<Post>();
    private readonly IGenericRepository<Vote> _voteRepository = unitOfWork.GetRepository<Vote>();

    public async Task<PostVoteResponseDto> ExecuteAsync(Guid currentUserId, Guid postId, CancellationToken ct = default)
    {
        if (currentUserId == Guid.Empty)
            throw new ArgumentException("Current user ID is required.");

        if (postId == Guid.Empty)
            throw new ArgumentException("Post ID is required.");

        var post = await _postRepository.GetQueryable()
            .FirstOrDefaultAsync(p => p.Id == postId && p.DeletedAt == null, ct);

        if (post is null)
            throw new KeyNotFoundException("Post not found.");

        var existingVote = await _voteRepository.GetQueryable()
            .FirstOrDefaultAsync(v => v.UserId == currentUserId && v.TargetId == postId && v.TargetType == "post", ct);

        bool isUpvoted;
        string message;

        if (existingVote is null)
        {
            await _voteRepository.AddAsync(new Vote
            {
                Id = Guid.NewGuid(),
                UserId = currentUserId,
                TargetId = postId,
                TargetType = "post",
                VoteType = "up",
                CreatedAt = DateTime.UtcNow
            }, ct);

            post.Upvotes += 1;
            isUpvoted = true;
            message = "Post upvoted successfully.";
        }
        else if (existingVote.VoteType == "up")
        {
            await _voteRepository.DeleteAsync(existingVote, ct);
            post.Upvotes = Math.Max(0, post.Upvotes - 1);
            isUpvoted = false;
            message = "Post upvote removed successfully.";
        }
        else
        {
            existingVote.VoteType = "up";
            await _voteRepository.UpdateAsync(existingVote, ct);
            post.Upvotes += 2;
            isUpvoted = true;
            message = "Post upvoted successfully.";
        }

        post.UpdatedAt = DateTime.UtcNow;
        await _postRepository.UpdateAsync(post, ct);
        await unitOfWork.SaveChangesAsync(ct);

        return new PostVoteResponseDto(
            PostId: post.Id,
            Upvotes: post.Upvotes,
            IsUpvoted: isUpvoted,
            Message: message
        );
    }
}
