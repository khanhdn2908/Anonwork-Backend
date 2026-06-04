using Anonwork.Application.Interfaces;
using Anonwork.Domain.Entities;

namespace Anonwork.Application.Features.Roles;

public class DeleteRoleUseCase(IUnitOfWork unitOfWork)
{
    private readonly IGenericRepository<Role> _roleRepo = unitOfWork.GetRepository<Role>();

    public async Task ExecuteAsync(Guid id, CancellationToken ct = default)
    {
        var role = await _roleRepo.GetByIdWithTrackingAsync(id, ct)
            ?? throw new KeyNotFoundException($"Role with id '{id}' not found.");

        await _roleRepo.DeleteAsync(role, ct);
        await unitOfWork.SaveChangesAsync(ct);
    }
}
