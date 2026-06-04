using Anonwork.Application.Features.Roles.DTOs;
using Anonwork.Application.Interfaces;
using Anonwork.Domain.Entities;

namespace Anonwork.Application.Features.Roles;

public class GetAllRolesUseCase(IUnitOfWork unitOfWork)
{
    private readonly IGenericRepository<Role> _roleRepo = unitOfWork.GetRepository<Role>();

    public async Task<IReadOnlyCollection<RoleDto>> ExecuteAsync(CancellationToken ct = default)
    {
        var roles = await _roleRepo.GetAllAsync(ct);
        return roles
            .OrderByDescending(r => r.CreatedAt)
            .Select(Map)
            .ToList();
    }

    private static RoleDto Map(Role role) => new(role.Id, role.Name, role.Description, role.IsActive, role.CreatedAt, role.UpdatedAt);
}
