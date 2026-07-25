using Anonwork.Application.Features.Comments.DTOs.Responses;
using Anonwork.Application.Interfaces;
using Anonwork.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Anonwork.Application.Features.Comments;

/// <summary>
/// Use case for getting comments by post with pagination
/// </summary>
public class GetCommentsByPostUseCase(IUnitOfWork unitOfWork, IR2Service r2Service)
{
    private readonly IGenericRepository<Comment> _commentRepository = unitOfWork.GetRepository<Comment>();
    private readonly IR2Service _r2Service = r2Service;

    public async Task<CommentListResponseDto> ExecuteAsync(
        Guid postId,
        bool hasPermission,
        int page = 1,
        int pageSize = 10,
        CancellationToken ct = default)
    {
        // ── Validation ──────────────────────────────
        if (postId == Guid.Empty)
            throw new ArgumentException("Post ID is required.");

        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 10;
        if (pageSize > 100) pageSize = 100; // Max 100 per page

        // ── Get comments ────────────────────────────
        var query = _commentRepository.GetQueryableNoTracking()
            .Include(c => c.Author)
                .ThenInclude(a => a != null ? a.AnonImage : null)
            .Include(c => c.Parent)
            .Where(c => c.PostId == postId);

        if (!hasPermission)
        {
            query = query.Where(c => c.IsActive);
        }

        query = query
            .OrderBy(c => c.Depth)
            .ThenBy(c => c.CreatedAt);

        var total = await query.CountAsync(ct);
        var comments = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        // ── Calculate pagination ────────────────────
        var totalPages = (int)Math.Ceiling((double)total / pageSize);

        // ── Return response ─────────────────────────
        var commentDtos = comments.Select(MapToResponse).ToList();
        return new CommentListResponseDto(commentDtos, total, page, pageSize, totalPages);
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
