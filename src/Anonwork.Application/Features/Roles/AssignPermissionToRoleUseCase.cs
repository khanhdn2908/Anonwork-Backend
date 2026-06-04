using Anonwork.Application.Interfaces;
using Anonwork.Domain.Entities;

namespace Anonwork.Application.Features.Roles;

public class AssignPermissionToRoleUseCase(IUnitOfWork unitOfWork)
{
    private readonly IGenericRepository<Role> _roleRepo = unitOfWork.GetRepository<Role>();
    private readonly IGenericRepository<Permission> _permissionRepo = unitOfWork.GetRepository<Permission>();
    private readonly IGenericRepository<RolePermission> _rolePermissionRepo = unitOfWork.GetRepository<RolePermission>();

    public async Task ExecuteAsync(Guid roleId, Guid permissionId, CancellationToken ct = default)
    {
        var roleExists = await _roleRepo.ExistsAsync(roleId, ct);
        if (!roleExists)
            throw new KeyNotFoundException($"Role with id '{roleId}' not found.");

        var permissionExists = await _permissionRepo.ExistsAsync(permissionId, ct);
        if (!permissionExists)
            throw new KeyNotFoundException($"Permission with id '{permissionId}' not found.");

        var existing = await _rolePermissionRepo.FindSingleAsync(rp => rp.RoleId == roleId && rp.PermissionId == permissionId, ct);
        if (existing is not null)
            return;

        var rolePermission = new RolePermission
        {
            RoleId = roleId,
            PermissionId = permissionId,
            CreatedAt = DateTime.UtcNow
        };

        await _rolePermissionRepo.AddAsync(rolePermission, ct);
        await unitOfWork.SaveChangesAsync(ct);
    }
}
