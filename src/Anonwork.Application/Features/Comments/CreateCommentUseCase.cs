using Anonwork.Application.Features.Comments.DTOs.Requests;
using Anonwork.Application.Features.Comments.DTOs.Responses;
using Anonwork.Application.Interfaces;
using Anonwork.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Anonwork.Application.Features.Comments;

/// <summary>
/// Use case for creating a comment
/// </summary>
public class CreateCommentUseCase(IUnitOfWork unitOfWork, IR2Service r2Service)
{
    private readonly IGenericRepository<Comment> _commentRepository = unitOfWork.GetRepository<Comment>();
    private readonly IGenericRepository<Post> _postRepository = unitOfWork.GetRepository<Post>();
    private readonly IGenericRepository<User> _userRepository = unitOfWork.GetRepository<User>();
    private readonly IR2Service _r2Service = r2Service;

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
        var user = (await _userRepository.FindAsync(u => u.Id == currentUserId, ct)).FirstOrDefault();
        if (user == null)
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
        var isAnon = request.IsAnonymous || user.IsAnonDefault;

        var comment = new Comment
        {
            Id = Guid.NewGuid(),
            PostId = request.PostId,
            AuthorId = currentUserId,
            ParentId = request.ParentId,
            IsAnonymous = isAnon,
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
                .ThenInclude(a => a != null ? a.AnonImage : null)
            .Include(c => c.Parent)
            .FirstOrDefaultAsync(c => c.Id == createdComment.Id, ct);

        if (commentWithRelations == null)
            throw new InvalidOperationException("Failed to retrieve created comment.");

        return MapToResponse(commentWithRelations);
    }

    private CommentResponseDto MapToResponse(Comment comment)
    {
        var isAnon = comment.IsAnonymous || (comment.Author != null && comment.Author.IsAnonDefault);
        var displayUsername = isAnon ? (comment.Author?.AnonAlias ?? "Ẩn danh") : (comment.Author?.Username ?? "Unknown");
        var avatarKey = isAnon ? comment.Author?.AnonImage?.FileKey : comment.Author?.AvatarKey;
        var avatarUrl = string.IsNullOrWhiteSpace(avatarKey)
            ? _r2Service.GetPublicUrl("avatars/null.jpg")
            : _r2Service.GetPublicUrl(avatarKey);

        return new CommentResponseDto(
            Id: comment.Id,
            PostId: comment.PostId,
            AuthorId: comment.AuthorId,
            ParentId: comment.ParentId,
            IsAnonymous: isAnon,
            Content: comment.Content,
            Upvotes: comment.Upvotes,
            Depth: comment.Depth,
            IsActive: comment.IsActive,
            CreatedAt: comment.CreatedAt,
            UpdatedAt: comment.UpdatedAt,
            Author: comment.Author == null ? null : new CommentAuthorDto(
                Id: comment.Author.Id,
                Username: displayUsername,
                AnonAlias: comment.Author.AnonAlias,
                AvatarUrl: avatarUrl
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
