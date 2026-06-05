using Anonwork.Application.Interfaces;
using Anonwork.Domain.Entities;

namespace Anonwork.Application.Features.Bookmarks;

/// <summary>
/// Use case for deleting a bookmark
/// </summary>
public class DeleteBookmarkUseCase(IUnitOfWork unitOfWork)
{
    private readonly IGenericRepository<Bookmark> _bookmarkRepository = unitOfWork.GetRepository<Bookmark>();

    public async Task ExecuteAsync(Guid currentUserId, Guid postId, CancellationToken ct = default)
    {
        // ── Validate input ──────────────────────────
        if (currentUserId == Guid.Empty)
            throw new ArgumentException("Current user ID is required.");

        if (postId == Guid.Empty)
            throw new ArgumentException("Post ID is required.");

        // ── Find bookmark ───────────────────────────
        var bookmark = await _bookmarkRepository.FindSingleWithTrackingAsync(
            b => b.UserId == currentUserId && b.PostId == postId,
            ct);

        if (bookmark is null)
            throw new KeyNotFoundException("Bookmark not found.");

        // ── Delete bookmark ─────────────────────────
        await _bookmarkRepository.DeleteAsync(bookmark, ct);
        await unitOfWork.SaveChangesAsync(ct);
    }
}