using Anonwork.Application.Interfaces;
using Anonwork.Domain.Entities;

namespace Anonwork.Application.Features.Roles;

public class RemovePermissionFromRoleUseCase(IUnitOfWork unitOfWork)
{
    private readonly IGenericRepository<RolePermission> _rolePermissionRepo = unitOfWork.GetRepository<RolePermission>();

    public async Task ExecuteAsync(Guid roleId, Guid permissionId, CancellationToken ct = default)
    {
        var rolePermission = await _rolePermissionRepo.FindSingleWithTrackingAsync(rp => rp.RoleId == roleId && rp.PermissionId == permissionId, ct)
            ?? throw new KeyNotFoundException($"Permission '{permissionId}' is not assigned to role '{roleId}'.");

        await _rolePermissionRepo.DeleteAsync(rolePermission, ct);
        await unitOfWork.SaveChangesAsync(ct);
    }
}
