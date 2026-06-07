using Anonwork.Application.Features.Permissions.DTOs.Requests;
using Anonwork.Application.Interfaces;
using Anonwork.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Anonwork.Application.Features.Permissions;

public class GetAllPermissionsUseCase(IUnitOfWork unitOfWork)
{
    private readonly IGenericRepository<Permission> _permissionRepo = unitOfWork.GetRepository<Permission>();

    public async Task<IReadOnlyCollection<PermissionDto>> ExecuteAsync(
        string? searchTerm = null,
        bool? isActive = null,
        CancellationToken ct = default)
    {
        var query = _permissionRepo.GetQueryableNoTracking();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.Trim().ToLower();
            query = query.Where(p => p.Code.ToLower().Contains(term) ||
                                     (p.Description != null && p.Description.ToLower().Contains(term)));
        }

        if (isActive.HasValue)
        {
            query = query.Where(p => p.IsActive == isActive.Value);
        }

        var permissions = await query
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync(ct);

        return permissions.Select(Map).ToList();
    }

    private static PermissionDto Map(Permission permission) => new(permission.Id, permission.Code, permission.Description, permission.IsActive, permission.CreatedAt, permission.UpdatedAt);
}
