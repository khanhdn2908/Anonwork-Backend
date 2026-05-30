using Anonwork.Application.Interfaces;
using Anonwork.Infrastructure.Repositories;

namespace Anonwork.Infrastructure;

/// <summary>
/// Unit of Work implementation to coordinate repository operations
/// and persist changes through a single DbContext instance.
/// </summary>
public sealed class UnitOfWork : IUnitOfWork
{
    private readonly IAppDbContext _context;
    private readonly Dictionary<Type, object> _repositories = new();

    public UnitOfWork(IAppDbContext context)
    {
        _context = context;
    }

    public IGenericRepository<T> GetRepository<T>() where T : class
    {
        var type = typeof(T);

        if (_repositories.TryGetValue(type, out var repository))
        {
            return (IGenericRepository<T>)repository;
        }

        var newRepository = new GenericRepository<T>(_context);
        _repositories[type] = newRepository;

        return newRepository;
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _context.SaveChangesAsync(cancellationToken);
    }

    public void Dispose()
    {
        //GC.SuppressFinalize(this);
    }
}
