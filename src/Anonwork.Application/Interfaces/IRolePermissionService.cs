namespace Anonwork.Application.Interfaces;

public interface IRolePermissionService
{
    Task<IReadOnlyCollection<string>> GetPermissionCodesAsync(Guid userId, CancellationToken ct = default);
}
