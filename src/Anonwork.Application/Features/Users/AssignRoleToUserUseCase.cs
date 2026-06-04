using Anonwork.Application.Interfaces;
using Anonwork.Domain.Entities;

namespace Anonwork.Application.Features.Users;

public class AssignRoleToUserUseCase(IUnitOfWork unitOfWork)
{
    private readonly IGenericRepository<User> _userRepo = unitOfWork.GetRepository<User>();
    private readonly IGenericRepository<Role> _roleRepo = unitOfWork.GetRepository<Role>();
    private readonly IGenericRepository<UserRole> _userRoleRepo = unitOfWork.GetRepository<UserRole>();

    public async Task ExecuteAsync(Guid userId, Guid roleId, CancellationToken ct = default)
    {
        var userExists = await _userRepo.ExistsAsync(userId, ct);
        if (!userExists)
            throw new KeyNotFoundException($"User with id '{userId}' not found.");

        var roleExists = await _roleRepo.ExistsAsync(roleId, ct);
        if (!roleExists)
            throw new KeyNotFoundException($"Role with id '{roleId}' not found.");

        var alreadyAssigned = await _userRoleRepo.ExistsAsync(ur => ur.UserId == userId && ur.RoleId == roleId, ct);
        if (alreadyAssigned)
            return;

        await _userRoleRepo.AddAsync(new UserRole
        {
            UserId = userId,
            RoleId = roleId,
            CreatedAt = DateTime.UtcNow
        }, ct);

        await unitOfWork.SaveChangesAsync(ct);
    }
}
