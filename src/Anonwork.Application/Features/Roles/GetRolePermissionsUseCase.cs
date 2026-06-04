using Anonwork.Application.Interfaces;
using Anonwork.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Anonwork.Application.Features.Roles;

public record RolePermissionDto(Guid PermissionId, string Code, string? Description, bool IsActive);

public class GetRolePermissionsUseCase(IUnitOfWork unitOfWork)
{
    private readonly IGenericRepository<Role> _roleRepo = unitOfWork.GetRepository<Role>();

    public async Task<IReadOnlyCollection<RolePermissionDto>> ExecuteAsync(Guid roleId, CancellationToken ct = default)
    {
        var roleExists = await _roleRepo.ExistsAsync(roleId, ct);
        if (!roleExists)
            throw new KeyNotFoundException($"Role with id '{roleId}' not found.");

        var rolePermissions = await _roleRepo
            .GetQueryableNoTracking()
            .Where(r => r.Id == roleId)
            .SelectMany(r => r.RolePermissions)
            .Select(rp => new RolePermissionDto(
                rp.PermissionId,
                rp.Permission.Code,
                rp.Permission.Description,
                rp.Permission.IsActive))
            .ToListAsync(ct);

        return rolePermissions;
    }
}
