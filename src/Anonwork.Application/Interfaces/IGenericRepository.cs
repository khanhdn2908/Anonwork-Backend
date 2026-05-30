using System.Linq.Expressions;

namespace Anonwork.Application.Interfaces;

/// <summary>
/// Generic repository interface providing common CRUD operations
/// </summary>
/// <typeparam name="T">Entity type</typeparam>
public interface IGenericRepository<T> where T : class
{
    // ──────────────────────────────────────────
    // READ OPERATIONS
    // ──────────────────────────────────────────

    Task<T?> GetByIdAsync(Guid id, CancellationToken ct = default);

    Task<T?> GetByIdWithTrackingAsync(Guid id, CancellationToken ct = default);

    Task<IEnumerable<T>> GetAllAsync(CancellationToken ct = default);

    Task<IEnumerable<T>> GetPagedAsync(int page, int pageSize, CancellationToken ct = default);

    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default);

    Task<T?> FindSingleAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default);

    Task<T?> FindSingleWithTrackingAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default);

    Task<IEnumerable<T>> GetWithIncludesAsync(params Expression<Func<T, object>>[] includes);

    Task<IEnumerable<T>> FindWithIncludesAsync(
        Expression<Func<T, bool>> predicate,
        params Expression<Func<T, object>>[] includes);

    Task<int> CountAsync(CancellationToken ct = default);

    Task<int> CountAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default);

    Task<bool> ExistsAsync(Guid id, CancellationToken ct = default);

    Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default);

    // ──────────────────────────────────────────
    // WRITE OPERATIONS
    // ──────────────────────────────────────────

    Task<T> AddAsync(T entity, CancellationToken ct = default);

    Task AddRangeAsync(IEnumerable<T> entities, CancellationToken ct = default);

    Task UpdateAsync(T entity, CancellationToken ct = default);

    Task UpdateRangeAsync(IEnumerable<T> entities, CancellationToken ct = default);

    Task DeleteAsync(Guid id, CancellationToken ct = default);

    Task DeleteAsync(T entity, CancellationToken ct = default);

    Task DeleteRangeAsync(IEnumerable<T> entities, CancellationToken ct = default);

    Task DeleteWhereAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default);

    // ──────────────────────────────────────────
    // QUERY OPERATIONS
    // ──────────────────────────────────────────

    IQueryable<T> GetQueryable();

    IQueryable<T> GetQueryableNoTracking();
}