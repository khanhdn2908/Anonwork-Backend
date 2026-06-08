using Anonwork.Application.Features.Comments.DTOs.Requests;
using Anonwork.Application.Features.Comments.DTOs.Responses;
using Anonwork.Application.Interfaces;
using Anonwork.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Anonwork.Application.Features.Comments;

/// <summary>
/// Use case for creating a comment
/// </summary>
public class CreateCommentUseCase(IUnitOfWork unitOfWork)
{
    private readonly IGenericRepository<Comment> _commentRepository = unitOfWork.GetRepository<Comment>();
    private readonly IGenericRepository<Post> _postRepository = unitOfWork.GetRepository<Post>();
    private readonly IGenericRepository<User> _userRepository = unitOfWork.GetRepository<User>();

    public async Task<CommentResponseDto> ExecuteAsync(Guid currentUserId, CreateCommentRequest request, CancellationToken ct = default)
    {
        // ── Validate input ──────────────────────────
        if (currentUserId == Guid.Empty)
            throw new ArgumentException("Current user ID is required.");

        if (request.PostId == Guid.Empty)
            throw new ArgumentException("Post ID is required.");

        if (string.IsNullOrWhiteSpace(request.Content))
            throw new ArgumentException("Comment content is required.");

        // ── Check post exists ───────────────────────
        var postExists = await _postRepository.ExistsAsync(request.PostId, ct);
        if (!postExists)
            throw new KeyNotFoundException("Post not found.");

        // ── Check user exists ───────────────────────
        var userExists = await _userRepository.ExistsAsync(currentUserId, ct);
        if (!userExists)
            throw new KeyNotFoundException("User not found.");

        // ── Validate parent comment if provided ─────
        Comment? parentComment = null;
        if (request.ParentId.HasValue)
        {
            parentComment = await _commentRepository.GetQueryableNoTracking()
                .FirstOrDefaultAsync(c => c.Id == request.ParentId.Value && c.PostId == request.PostId, ct);

            if (parentComment is null)
                throw new KeyNotFoundException("Parent comment not found.");
        }

        // ── Create comment ──────────────────────────
        var comment = new Comment
        {
            Id = Guid.NewGuid(),
            PostId = request.PostId,
            AuthorId = currentUserId,
            ParentId = request.ParentId,
            IsAnonymous = request.IsAnonymous,
            Content = request.Content.Trim(),
            Upvotes = 0,
            Depth = parentComment == null ? 0 : parentComment.Depth + 1,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        };

        var createdComment = await _commentRepository.AddAsync(comment, ct);
        await unitOfWork.SaveChangesAsync(ct);

        // ── Load created comment with relations ─────
        var commentWithRelations = await _commentRepository.GetQueryableNoTracking()
            .Include(c => c.Author)
            .Include(c => c.Parent)
            .FirstOrDefaultAsync(c => c.Id == createdComment.Id, ct);

        if (commentWithRelations == null)
            throw new InvalidOperationException("Failed to retrieve created comment.");

        return MapToResponse(commentWithRelations);
    }

    private static CommentResponseDto MapToResponse(Comment comment)
    {
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
            UpdatedAt: comment.UpdatedAt,
            Author: comment.Author == null ? null : new CommentAuthorDto(
                Id: comment.Author.Id,
                Username: comment.Author.Username,
                AnonAlias: comment.Author.AnonAlias,
                AvatarUrl: comment.Author.AvatarUrl
            ),
            Parent: comment.Parent == null ? null : new CommentParentDto(
                Id: comment.Parent.Id,
                Content: comment.Parent.Content,
                AuthorId: comment.Parent.AuthorId,
                IsAnonymous: comment.Parent.IsAnonymous,
                CreatedAt: comment.Parent.CreatedAt
            )
        );
    }
}
