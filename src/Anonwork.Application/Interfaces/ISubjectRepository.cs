using Anonwork.Domain.Entities;

namespace Anonwork.Application.Interfaces;

/// <summary>
/// Repository interface for Subject entity
/// </summary>
public interface ISubjectRepository
{
    /// <summary>
    /// Get subject by id
    /// </summary>
    Task<Subject?> GetByIdAsync(Guid id, CancellationToken ct = default);

    /// <summary>
    /// Get all subjects with search and pagination
    /// </summary>
    Task<(List<Subject> Subjects, int Total)> GetAllAsync(
        string? searchQuery = null,
        int page = 1,
        int pageSize = 10,
        CancellationToken ct = default);

    /// <summary>
    /// Create a new subject
    /// </summary>
    Task<Subject> CreateAsync(Subject subject, CancellationToken ct = default);

    /// <summary>
    /// Update an existing subject
    /// </summary>
    Task UpdateAsync(Subject subject, CancellationToken ct = default);

    /// <summary>
    /// Delete a subject
    /// </summary>
    Task DeleteAsync(Guid id, CancellationToken ct = default);

    /// <summary>
    /// Check if subject exists
    /// </summary>
    Task<bool> ExistsByIdAsync(Guid id, CancellationToken ct = default);

    /// <summary>
    /// Check if subject exists by slug
    /// </summary>
    Task<bool> ExistsBySlugAsync(string slug, CancellationToken ct = default);

    /// <summary>
    /// Get subject by slug
    /// </summary>
    Task<Subject?> GetBySlugAsync(string slug, CancellationToken ct = default);
}
