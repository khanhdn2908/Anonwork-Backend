using Anonwork.Application.Features.Comments.DTOs;
using Anonwork.Application.Interfaces;
using Anonwork.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Anonwork.Application.Features.Comments;

/// <summary>
/// Use case for getting comments by post with pagination
/// </summary>
public class GetCommentsByPostUseCase(IUnitOfWork unitOfWork)
{
    private readonly IGenericRepository<Comment> _commentRepository = unitOfWork.GetRepository<Comment>();

    public async Task<CommentListResponseDto> ExecuteAsync(
        Guid postId,
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
            .Include(c => c.Parent)
            .Where(c => c.PostId == postId && !c.IsDeleted)
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
            IsDeleted: comment.IsDeleted,
            DeletedAt: comment.DeletedAt,
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
