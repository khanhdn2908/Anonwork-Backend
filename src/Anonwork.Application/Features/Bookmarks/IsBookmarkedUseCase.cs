using Anonwork.Application.Interfaces;
using Anonwork.Domain.Entities;

namespace Anonwork.Application.Features.Bookmarks;

/// <summary>
/// Use case for checking whether a post is bookmarked by current user
/// </summary>
public class IsBookmarkedUseCase(IUnitOfWork unitOfWork)
{
    private readonly IGenericRepository<Bookmark> _bookmarkRepository = unitOfWork.GetRepository<Bookmark>();

    public async Task<bool> ExecuteAsync(Guid currentUserId, Guid postId, CancellationToken ct = default)
    {
        // ── Validate input ──────────────────────────
        if (currentUserId == Guid.Empty)
            throw new ArgumentException("Current user ID is required.");

        if (postId == Guid.Empty)
            throw new ArgumentException("Post ID is required.");

        return await _bookmarkRepository.ExistsAsync(
            b => b.UserId == currentUserId && b.PostId == postId,
            ct);
    }
}