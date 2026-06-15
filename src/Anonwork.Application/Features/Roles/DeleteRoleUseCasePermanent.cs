using Anonwork.Application.Common.Exceptions;
using Anonwork.Application.Interfaces;
using Anonwork.Domain.Entities;

namespace Anonwork.Application.Features.Roles;

public class DeleteRoleUseCasePermanent(IUnitOfWork unitOfWork)
{
    private readonly IGenericRepository<Role> _roleRepo = unitOfWork.GetRepository<Role>();

    public async Task ExecuteAsync(Guid id, CancellationToken ct = default)
    {
        if (id == Guid.Empty)
            throw new ArgumentException("Role id is required.");

        var role = await _roleRepo.GetByIdAsync(id, ct)
            ?? throw new NotFoundException(nameof(Role), id);

        if (role.IsActive)
            throw new ArgumentException("Role need deleted first.");

        await _roleRepo.DeleteAsync(id, ct);
        await unitOfWork.SaveChangesAsync(ct);
    }
}
