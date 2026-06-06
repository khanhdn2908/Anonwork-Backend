using Anonwork.Application.Features.Permissions.DTOs.Requests;
using Anonwork.Application.Interfaces;
using Anonwork.Domain.Entities;

namespace Anonwork.Application.Features.Permissions;

public class CreatePermissionUseCase(IUnitOfWork unitOfWork)
{
    private readonly IGenericRepository<Permission> _permissionRepo = unitOfWork.GetRepository<Permission>();

    public async Task<PermissionDto> ExecuteAsync(PermissionRequestDto request, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(request.Code))
            throw new ArgumentException("Permission code is required.");

        var code = request.Code.Trim().ToLowerInvariant();
        var existing = await _permissionRepo.FindSingleAsync(p => p.Code == code, ct);
        if (existing is not null)
            throw new InvalidOperationException($"Permission with code '{request.Code}' already exists.");

        var permission = Permission.Create(code, request.Description);
        permission.IsActive = request.IsActive;

        var created = await _permissionRepo.AddAsync(permission, ct);
        await unitOfWork.SaveChangesAsync(ct);

        return Map(created);
    }

    private static PermissionDto Map(Permission permission) => new(permission.Id, permission.Code, permission.Description, permission.IsActive, permission.CreatedAt, permission.UpdatedAt);
}
