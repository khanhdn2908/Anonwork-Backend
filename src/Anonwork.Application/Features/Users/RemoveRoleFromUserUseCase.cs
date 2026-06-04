using Anonwork.Application.Interfaces;
using Anonwork.Domain.Entities;

namespace Anonwork.Application.Features.Users;

public class RemoveRoleFromUserUseCase(IUnitOfWork unitOfWork)
{
    private readonly IGenericRepository<UserRole> _userRoleRepo = unitOfWork.GetRepository<UserRole>();

    public async Task ExecuteAsync(Guid userId, Guid roleId, CancellationToken ct = default)
    {
        var userRole = await _userRoleRepo.FindSingleWithTrackingAsync(ur => ur.UserId == userId && ur.RoleId == roleId, ct)
            ?? throw new KeyNotFoundException($"Role '{roleId}' is not assigned to user '{userId}'.");

        await _userRoleRepo.DeleteAsync(userRole, ct);
        await unitOfWork.SaveChangesAsync(ct);
    }
}
