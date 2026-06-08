using Anonwork.Application.Features.Comments.DTOs.Responses;
using Anonwork.Application.Interfaces;
using Anonwork.Domain.Entities;

namespace Anonwork.Application.Features.Comments;

/// <summary>
/// Use case for updating a comment
/// </summary>
public class UpdateCommentUseCase(IUnitOfWork unitOfWork)
{
    private readonly IGenericRepository<Comment> _commentRepository = unitOfWork.GetRepository<Comment>();

    public async Task<CommentResponseDto> ExecuteAsync(Guid currentUserId, Guid commentId, string content, CancellationToken ct = default)
    {
        // ── Validate input ──────────────────────────
        if (currentUserId == Guid.Empty)
            throw new ArgumentException("Current user ID is required.");

        if (commentId == Guid.Empty)
            throw new ArgumentException("Comment ID is required.");

        if (string.IsNullOrWhiteSpace(content))
            throw new ArgumentException("Comment content is required.");

        // ── Find comment ────────────────────────────
        var comment = await _commentRepository.FindSingleWithTrackingAsync(c => c.Id == commentId, ct);
        if (comment is null)
            throw new KeyNotFoundException("Comment not found.");

        if (comment.AuthorId != currentUserId)
            throw new UnauthorizedAccessException("You can only update your own comments.");

        if (!comment.IsActive)
            throw new InvalidOperationException("Cannot update a deleted comment.");

        // ── Update comment ──────────────────────────
        comment.Content = content.Trim();
        comment.UpdatedAt = DateTime.UtcNow;

        await unitOfWork.SaveChangesAsync(ct);

        return new CommentResponseDto(
            Id: comment.Id,
            PostId: comment.PostId,
            AuthorId: comment.AuthorId,
            ParentId: comment.ParentId,
            IsAnonymous: comment.IsAnonymous,
            Content: comment.Content,
            Upvotes: comment.Upvotes,
            Depth: comment.Depth,
            IsActive: comment.IsActive,
            CreatedAt: comment.CreatedAt,
            UpdatedAt: comment.UpdatedAt
        );
    }
}
