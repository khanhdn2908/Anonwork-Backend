using Anonwork.Application.Features.Bookmarks.DTOs.Requests;
using Anonwork.Application.Features.Bookmarks.DTOs.Responses;
using Anonwork.Application.Interfaces;
using Anonwork.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Anonwork.Application.Features.Bookmarks;

/// <summary>
/// Use case for creating a bookmark
/// </summary>
public class CreateBookmarkUseCase(IUnitOfWork unitOfWork)
{
    private readonly IGenericRepository<Bookmark> _bookmarkRepository = unitOfWork.GetRepository<Bookmark>();
    private readonly IGenericRepository<Post> _postRepository = unitOfWork.GetRepository<Post>();

    public async Task<BookmarkResponseDto> ExecuteAsync(Guid currentUserId, CreateBookmarkRequest request, CancellationToken ct = default)
    {
        // ── Validate input ──────────────────────────
        if (currentUserId == Guid.Empty)
            throw new ArgumentException("Current user ID is required.");

        if (request.PostId == Guid.Empty)
            throw new ArgumentException("Post ID is required.");

        // ── Check post exists ───────────────────────
        var postExists = await _postRepository.ExistsAsync(request.PostId, ct);
        if (!postExists)
            throw new KeyNotFoundException("Post not found.");

        // ── Check already bookmarked ────────────────
        var alreadyBookmarked = await _bookmarkRepository.ExistsAsync(
            b => b.UserId == currentUserId && b.PostId == request.PostId,
            ct);

        if (alreadyBookmarked)
            throw new InvalidOperationException("Post is already bookmarked.");

        // ── Create bookmark ─────────────────────────
        var bookmark = new Bookmark
        {
            Id = Guid.NewGuid(),
            UserId = currentUserId,
            PostId = request.PostId,
            CreatedAt = DateTime.UtcNow,
        };

        var createdBookmark = await _bookmarkRepository.AddAsync(bookmark, ct);
        await unitOfWork.SaveChangesAsync(ct);

        // ── Load created bookmark with post data ────
        var bookmarkWithPost = await _bookmarkRepository.GetQueryableNoTracking()
            .Include(b => b.Post)
                .ThenInclude(p => p.Author)
            .Include(b => b.Post)
                .ThenInclude(p => p.Subject)
            .Include(b => b.Post)
                .ThenInclude(p => p.PostMediaItems)
            .Include(b => b.Post)
                .ThenInclude(p => p.PostTags)
            .FirstOrDefaultAsync(b => b.Id == createdBookmark.Id, ct);

        if (bookmarkWithPost == null)
            throw new InvalidOperationException("Failed to retrieve created bookmark.");

        return MapToResponse(bookmarkWithPost);
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