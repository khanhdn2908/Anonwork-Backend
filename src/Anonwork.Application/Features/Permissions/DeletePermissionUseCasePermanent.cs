using Anonwork.Application.Common.Exceptions;
using Anonwork.Application.Interfaces;
using Anonwork.Domain.Entities;

namespace Anonwork.Application.Features.Permissions;

public class DeletePermissionUseCasePermanent(IUnitOfWork unitOfWork)
{
    private readonly IGenericRepository<Permission> _permissionRepo = unitOfWork.GetRepository<Permission>();

    public async Task ExecuteAsync(Guid id, CancellationToken ct = default)
    {
        if (id == Guid.Empty)
            throw new ArgumentException("Permission id is required.");

        var permission = await _permissionRepo.GetByIdAsync(id, ct)
            ?? throw new NotFoundException(nameof(Permission), id);

        if (permission.IsActive)
            throw new ArgumentException("Permission need deleted first");

        await _permissionRepo.DeleteAsync(id, ct);
        await unitOfWork.SaveChangesAsync(ct);
    }
}
