using Anonwork.Application.Features.Roles.DTOs;
using Anonwork.Application.Interfaces;
using Anonwork.Domain.Entities;

namespace Anonwork.Application.Features.Roles;

public class AssignPermissionsToRoleUseCase(IUnitOfWork unitOfWork)
{
    private readonly IGenericRepository<Role> _roleRepo = unitOfWork.GetRepository<Role>();
    private readonly IGenericRepository<Permission> _permissionRepo = unitOfWork.GetRepository<Permission>();
    private readonly IGenericRepository<RolePermission> _rolePermissionRepo = unitOfWork.GetRepository<RolePermission>();

    public async Task ExecuteAsync(Guid roleId, AssignPermissionsRequestDto request, CancellationToken ct = default)
    {
        if (request.PermissionIds is null || request.PermissionIds.Count == 0)
            throw new ArgumentException("PermissionIds cannot be empty.");

        var roleExists = await _roleRepo.ExistsAsync(roleId, ct);
        if (!roleExists)
            throw new KeyNotFoundException($"Role with id '{roleId}' not found.");

        var permissionIds = request.PermissionIds.Distinct().ToArray();
        var permissions = await _permissionRepo.FindAsync(p => permissionIds.Contains(p.Id), ct);
        var foundIds = permissions.Select(p => p.Id).ToHashSet();
        var missingIds = permissionIds.Where(id => !foundIds.Contains(id)).ToArray();
        if (missingIds.Length > 0)
            throw new KeyNotFoundException($"Permissions not found: {string.Join(", ", missingIds)}");

        var existing = await _rolePermissionRepo.FindAsync(rp => rp.RoleId == roleId, ct);
        var existingIds = existing.Select(rp => rp.PermissionId).ToHashSet();

        var toAdd = permissionIds
            .Where(id => !existingIds.Contains(id))
            .Select(id => new RolePermission
            {
                RoleId = roleId,
                PermissionId = id,
                CreatedAt = DateTime.UtcNow
            })
            .ToList();

        if (toAdd.Count == 0)
            return;

        await _rolePermissionRepo.AddRangeAsync(toAdd, ct);
        await unitOfWork.SaveChangesAsync(ct);
    }
}
