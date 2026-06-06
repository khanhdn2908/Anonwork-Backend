using Anonwork.Application.Features.Permissions.DTOs.Requests;
using Anonwork.Application.Interfaces;
using Anonwork.Domain.Entities;

namespace Anonwork.Application.Features.Permissions;

public class UpdatePermissionUseCase(IUnitOfWork unitOfWork)
{
    private readonly IGenericRepository<Permission> _permissionRepo = unitOfWork.GetRepository<Permission>();

    public async Task<PermissionDto> ExecuteAsync(Guid id, PermissionRequestDto request, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(request.Code))
            throw new ArgumentException("Permission code is required.");

        var permission = await _permissionRepo.GetByIdWithTrackingAsync(id, ct)
            ?? throw new KeyNotFoundException($"Permission with id '{id}' not found.");

        var code = request.Code.Trim().ToLowerInvariant();
        var duplicate = await _permissionRepo.FindSingleAsync(p => p.Code == code && p.Id != id, ct);
        if (duplicate is not null)
            throw new InvalidOperationException($"Permission with code '{request.Code}' already exists.");

        permission.Code = code;
        permission.Description = request.Description;
        permission.IsActive = request.IsActive;
        permission.UpdatedAt = DateTime.UtcNow;

        await _permissionRepo.UpdateAsync(permission, ct);
        await unitOfWork.SaveChangesAsync(ct);

        return Map(permission);
    }

    private static PermissionDto Map(Permission permission) => new(permission.Id, permission.Code, permission.Description, permission.IsActive, permission.CreatedAt, permission.UpdatedAt);
}
