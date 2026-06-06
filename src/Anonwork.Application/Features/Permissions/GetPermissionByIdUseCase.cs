using Anonwork.Application.Features.Permissions.DTOs.Requests;
using Anonwork.Application.Interfaces;
using Anonwork.Domain.Entities;

namespace Anonwork.Application.Features.Permissions;

public class GetPermissionByIdUseCase(IUnitOfWork unitOfWork)
{
    private readonly IGenericRepository<Permission> _permissionRepo = unitOfWork.GetRepository<Permission>();

    public async Task<PermissionDto> ExecuteAsync(Guid id, CancellationToken ct = default)
    {
        var permission = await _permissionRepo.GetByIdAsync(id, ct)
            ?? throw new KeyNotFoundException($"Permission with id '{id}' not found.");

        return Map(permission);
    }

    private static PermissionDto Map(Permission permission) => new(permission.Id, permission.Code, permission.Description, permission.IsActive, permission.CreatedAt, permission.UpdatedAt);
}
