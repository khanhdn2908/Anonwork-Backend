using Anonwork.Application.Features.Permissions.DTOs;
using Anonwork.Application.Interfaces;
using Anonwork.Domain.Entities;

namespace Anonwork.Application.Features.Permissions;

public class GetAllPermissionsUseCase(IUnitOfWork unitOfWork)
{
    private readonly IGenericRepository<Permission> _permissionRepo = unitOfWork.GetRepository<Permission>();

    public async Task<IReadOnlyCollection<PermissionDto>> ExecuteAsync(CancellationToken ct = default)
    {
        var permissions = await _permissionRepo.GetAllAsync(ct);
        return permissions
            .OrderByDescending(p => p.CreatedAt)
            .Select(Map)
            .ToList();
    }

    private static PermissionDto Map(Permission permission) => new(permission.Id, permission.Code, permission.Description, permission.IsActive, permission.CreatedAt, permission.UpdatedAt);
}
