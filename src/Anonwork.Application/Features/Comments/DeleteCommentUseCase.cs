using Anonwork.Application.Common.Exceptions;
using Anonwork.Application.Interfaces;
using Anonwork.Domain.Entities;

namespace Anonwork.Application.Features.Comments;

/// <summary>
/// Use case for deleting a comment
/// </summary>
public class DeleteCommentUseCase(IUnitOfWork unitOfWork)
{
    private readonly IGenericRepository<Comment> _commentRepository = unitOfWork.GetRepository<Comment>();

    public async Task ExecuteAsync(Guid currentUserId, Guid commentId, CancellationToken ct = default)
    {
        // ── Validate input ──────────────────────────
        if (currentUserId == Guid.Empty)
            throw new ArgumentException("Current user ID is required.");

        if (commentId == Guid.Empty)
            throw new ArgumentException("Comment ID is required.");

        // ── Find comment ────────────────────────────
        var comment = await _commentRepository.FindSingleWithTrackingAsync(c => c.Id == commentId, ct);
        if (comment is null)
            throw new NotFoundException(nameof(Comment), commentId);

        if (comment.AuthorId != currentUserId)
            throw new InvalidOperationException("You can only delete your own comments.");

        if (!comment.IsActive)
            return;

        // ── Soft delete comment ─────────────────────
        comment.IsActive = false;
        comment.UpdatedAt = DateTime.UtcNow;
        comment.Content = "[deleted]";

        await unitOfWork.SaveChangesAsync(ct);
    }
}
