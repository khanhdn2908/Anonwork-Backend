using Anonwork.Application.Features.Comments.DTOs.Responses;
using Anonwork.Application.Interfaces;
using Anonwork.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Anonwork.Application.Features.Comments;

/// <summary>
/// Use case for toggling a comment upvote
/// </summary>
public class ToggleCommentVoteUseCase(IUnitOfWork unitOfWork)
{
    private readonly IGenericRepository<Comment> _commentRepository = unitOfWork.GetRepository<Comment>();
    private readonly IGenericRepository<Vote> _voteRepository = unitOfWork.GetRepository<Vote>();

    public async Task<CommentVoteResponseDto> ExecuteAsync(Guid currentUserId, Guid commentId, CancellationToken ct = default)
    {
        // ── Validate input ──────────────────────────
        if (currentUserId == Guid.Empty)
            throw new ArgumentException("Current user ID is required.");

        if (commentId == Guid.Empty)
            throw new ArgumentException("Comment ID is required.");

        // ── Find comment ────────────────────────────
        var comment = await _commentRepository.GetQueryable()
            .FirstOrDefaultAsync(c => c.Id == commentId && !c.IsDeleted, ct);

        if (comment is null)
            throw new KeyNotFoundException("Comment not found.");

        // ── Find existing vote ──────────────────────
        var existingVote = await _voteRepository.GetQueryable()
            .FirstOrDefaultAsync(v => v.UserId == currentUserId && v.TargetId == commentId && v.TargetType == "comment", ct);

        bool isUpvoted;
        string message;

        if (existingVote is null)
        {
            await _voteRepository.AddAsync(new Vote
            {
                Id = Guid.NewGuid(),
                UserId = currentUserId,
                TargetId = commentId,
                TargetType = "comment",
                VoteType = "up",
                CreatedAt = DateTime.UtcNow
            }, ct);

            comment.Upvotes += 1;
            isUpvoted = true;
            message = "Comment upvoted successfully.";
        }
        else if (existingVote.VoteType == "up")
        {
            await _voteRepository.DeleteAsync(existingVote, ct);
            comment.Upvotes = Math.Max(0, comment.Upvotes - 1);
            isUpvoted = false;
            message = "Comment upvote removed successfully.";
        }
        else
        {
            existingVote.VoteType = "up";
            await _voteRepository.UpdateAsync(existingVote, ct);
            comment.Upvotes += 2;
            isUpvoted = true;
            message = "Comment upvoted successfully.";
        }

        comment.UpdatedAt = DateTime.UtcNow;
        await _commentRepository.UpdateAsync(comment, ct);
        await unitOfWork.SaveChangesAsync(ct);

        return new CommentVoteResponseDto(
            CommentId: comment.Id,
            Upvotes: comment.Upvotes,
            IsUpvoted: isUpvoted,
            Message: message
        );
    }
}
