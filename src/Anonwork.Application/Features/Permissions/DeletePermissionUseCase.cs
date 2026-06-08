using Anonwork.Application.Interfaces;
using Anonwork.Domain.Entities;

namespace Anonwork.Application.Features.Permissions;

public class DeletePermissionUseCase(IUnitOfWork unitOfWork)
{
    private readonly IGenericRepository<Permission> _permissionRepo = unitOfWork.GetRepository<Permission>();

    public async Task ExecuteAsync(Guid id, CancellationToken ct = default)
    {
        var permission = await _permissionRepo.GetByIdWithTrackingAsync(id, ct)
            ?? throw new KeyNotFoundException($"Permission with id '{id}' not found.");

        permission.IsActive = false;
        permission.UpdatedAt = DateTime.UtcNow;
        await unitOfWork.SaveChangesAsync(ct);
    }
}
