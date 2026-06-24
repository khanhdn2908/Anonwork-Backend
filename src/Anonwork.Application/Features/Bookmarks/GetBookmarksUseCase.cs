using Anonwork.Application.Features.Bookmarks.DTOs.Responses;
using Anonwork.Application.Interfaces;
using Anonwork.Domain.Entities;
using Anonwork.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Anonwork.Application.Features.Bookmarks;

/// <summary>
/// Use case for getting user bookmarks with pagination
/// </summary>
public class GetBookmarksUseCase(IUnitOfWork unitOfWork)
{
    private readonly IGenericRepository<Bookmark> _bookmarkRepository = unitOfWork.GetRepository<Bookmark>();

    public async Task<BookmarkListResponseDto> ExecuteAsync(
        Guid currentUserId,
        bool hasPermission,
        string? search = null,
        int page = 1,
        int pageSize = 10,
        CancellationToken ct = default)
    {
        // ── Validation ──────────────────────────────
        if (currentUserId == Guid.Empty)
            throw new ArgumentException("Current user ID is required.");

        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 10;
        if (pageSize > 100) pageSize = 100; // Max 100 per page

        search = string.IsNullOrWhiteSpace(search) ? null : search.Trim();

        // ── Get bookmarks ───────────────────────────
        var query = _bookmarkRepository.GetQueryableNoTracking()
            .Include(b => b.Post)
                .ThenInclude(p => p.Author)
            .Include(b => b.Post)
                .ThenInclude(p => p.Subject)
            .Include(b => b.Post)
                .ThenInclude(p => p.PostMediaItems)
            .Include(b => b.Post)
                .ThenInclude(p => p.PostTags)
            .Where(b => b.UserId == currentUserId);

        if(!hasPermission) query = query.Where(b => b.Post.Status == PostStatus.Published);

        if (search is not null)
        {
            query = query.Where(b =>
                b.Post != null && (
                    EF.Functions.Like(b.Post.Title, $"%{search}%") ||
                    EF.Functions.Like(b.Post.Content, $"%{search}%") ||
                    (b.Post.Author != null && (
                        EF.Functions.Like(b.Post.Author.Username, $"%{search}%") ||
                        EF.Functions.Like(b.Post.Author.AnonAlias, $"%{search}%")
                    )) ||
                    (b.Post.Subject != null && EF.Functions.Like(b.Post.Subject.Name, $"%{search}%")) ||
                    b.Post.PostTags.Any(pt => EF.Functions.Like(pt.Tag, $"%{search}%"))
                )
            );
        }

        query = query.OrderByDescending(b => b.CreatedAt);

        var total = await query.CountAsync(ct);
        var bookmarks = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        // ── Calculate pagination ────────────────────
        var totalPages = (int)Math.Ceiling((double)total / pageSize);

        // ── Return response ─────────────────────────
        var bookmarkDtos = bookmarks.Select(MapToResponse).ToList();
        return new BookmarkListResponseDto(bookmarkDtos, total, page, pageSize, totalPages);
    }

    private static BookmarkResponseDto MapToResponse(Bookmark bookmark)
    {
        return new BookmarkResponseDto(
            Id: bookmark.Id,
            UserId: bookmark.UserId,
            PostId: bookmark.PostId,
            CreatedAt: bookmark.CreatedAt,
            Post: bookmark.Post == null ? null : MapPostToDto(bookmark.Post)
        );
    }

    private static BookmarkPostDto MapPostToDto(Post post)
    {
        var media = post.PostMediaItems
            .OrderBy(pm => pm.DisplayOrder)
            .ToList();

        var previewImageUrls = media
            .Where(pm => pm.MediaType == Anonwork.Domain.Enums.PostMediaType.Image)
            .Select(pm => pm.FileKey)
            .Take(2)
            .ToList();

        var remainingImagesCount = media.Count(pm => pm.MediaType == Anonwork.Domain.Enums.PostMediaType.Image) - previewImageUrls.Count;
        remainingImagesCount = Math.Max(0, remainingImagesCount);

        return new BookmarkPostDto(
            Id: post.Id,
            Title: post.Title,
            Content: post.Content,
            AuthorId: post.AuthorId,
            AuthorUsername: post.Author?.Username,
            AuthorAnonAlias: post.Author?.AnonAlias,
            IsAnonymous: post.IsAnonymous,
            SubjectId: post.SubjectId,
            SubjectName: post.Subject?.Name,
            ImageUrls: previewImageUrls,
            RemainingImagesCount: remainingImagesCount,
            Tags: post.PostTags.Select(pt => pt.Tag).ToList(),
            Upvotes: post.Upvotes,
            CommentsCount: post.CommentsCount,
            ViewCount: post.ViewCount,
            Status: post.Status.ToString(),
            CreatedAt: post.CreatedAt,
            UpdatedAt: post.UpdatedAt
        );
    }
}