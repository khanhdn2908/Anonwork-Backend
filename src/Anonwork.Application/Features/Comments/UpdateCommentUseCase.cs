using Anonwork.Application.Features.Comments.DTOs.Responses;
using Anonwork.Application.Interfaces;
using Anonwork.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Anonwork.Application.Features.Comments;

/// <summary>
/// Use case for updating a comment
/// </summary>
public class UpdateCommentUseCase(IUnitOfWork unitOfWork, IR2Service r2Service)
{
    private readonly IGenericRepository<Comment> _commentRepository = unitOfWork.GetRepository<Comment>();
    private readonly IR2Service _r2Service = r2Service;

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

        var updatedComment = await _commentRepository.GetQueryableNoTracking()
            .Include(c => c.Author)
            .Include(c => c.Parent)
            .FirstOrDefaultAsync(c => c.Id == commentId, ct);

        if (updatedComment is null)
            throw new InvalidOperationException("Failed to retrieve updated comment.");

        return new CommentResponseDto(
            Id: updatedComment.Id,
            PostId: updatedComment.PostId,
            AuthorId: updatedComment.AuthorId,
            ParentId: updatedComment.ParentId,
            IsAnonymous: updatedComment.IsAnonymous,
            Content: updatedComment.Content,
            Upvotes: updatedComment.Upvotes,
            Depth: updatedComment.Depth,
            IsActive: updatedComment.IsActive,
            CreatedAt: updatedComment.CreatedAt,
            UpdatedAt: updatedComment.UpdatedAt,
            Author: updatedComment.Author == null ? null : new CommentAuthorDto(
                Id: updatedComment.Author.Id,
                Username: updatedComment.Author.Username,
                AnonAlias: updatedComment.Author.AnonAlias,
                AvatarUrl: updatedComment.Author.AvatarKey is null ? null : _r2Service.GetPublicUrl(updatedComment.Author.AvatarKey)
            ),
            Parent: updatedComment.Parent == null ? null : new CommentParentDto(
                Id: updatedComment.Parent.Id,
                Content: updatedComment.Parent.Content,
                AuthorId: updatedComment.Parent.AuthorId,
                IsAnonymous: updatedComment.Parent.IsAnonymous,
                CreatedAt: updatedComment.Parent.CreatedAt
            )
        );
    }
}
