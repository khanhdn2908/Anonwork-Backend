using Anonwork.Domain.Entities;

namespace Anonwork.Application.Interfaces;

/// <summary>
/// Repository interface for Post entity
/// </summary>
public interface IPostRepository
{
    /// <summary>
    /// Get post by id
    /// </summary>
    Task<Post?> GetByIdAsync(Guid id, CancellationToken ct = default);

    /// <summary>
    /// Get post by id with related data (author, subject, images, tags)
    /// </summary>
    Task<Post?> GetByIdWithDetailsAsync(Guid id, CancellationToken ct = default);

    /// <summary>
    /// Create a new post
    /// </summary>
    Task<Post> CreateAsync(Post post, CancellationToken ct = default);

    /// <summary>
    /// Update an existing post
    /// </summary>
    Task UpdateAsync(Post post, CancellationToken ct = default);

    /// <summary>
    /// Delete a post (soft delete)
    /// </summary>
    Task DeleteAsync(Guid id, CancellationToken ct = default);

    /// <summary>
    /// Check if post exists
    /// </summary>
    Task<bool> ExistsByIdAsync(Guid id, CancellationToken ct = default);

    /// <summary>
    /// Get posts by subject with pagination
    /// </summary>
    Task<(List<Post> Posts, int Total)> GetBySubjectAsync(
        Guid subjectId,
        int page = 1,
        int pageSize = 10,
        CancellationToken ct = default);

    /// <summary>
    /// Get all active posts with pagination
    /// </summary>
    Task<(List<Post> Posts, int Total)> GetAllAsync(
        int page = 1,
        int pageSize = 10,
        CancellationToken ct = default);

    /// <summary>
    /// Get posts by author
    /// </summary>
    Task<List<Post>> GetByAuthorAsync(Guid authorId, CancellationToken ct = default);

    /// <summary>
    /// Increment view count
    /// </summary>
    Task IncrementViewCountAsync(Guid postId, CancellationToken ct = default);
}
