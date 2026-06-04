using Anonwork.Application.Interfaces;
using Anonwork.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Anonwork.Application.Features.Users;

public record UserRoleDto(Guid RoleId, string Name, string? Description, bool IsActive);

public class GetUserRolesUseCase(IUnitOfWork unitOfWork)
{
    private readonly IGenericRepository<User> _userRepo = unitOfWork.GetRepository<User>();

    public async Task<IReadOnlyCollection<UserRoleDto>> ExecuteAsync(Guid userId, CancellationToken ct = default)
    {
        var userExists = await _userRepo.ExistsAsync(userId, ct);
        if (!userExists)
            throw new KeyNotFoundException($"User with id '{userId}' not found.");

        return await _userRepo
            .GetQueryableNoTracking()
            .Where(u => u.Id == userId)
            .SelectMany(u => u.UserRoles)
            .Select(ur => new UserRoleDto(
                ur.RoleId,
                ur.Role.Name,
                ur.Role.Description,
                ur.Role.IsActive))
            .ToListAsync(ct);
    }
}
