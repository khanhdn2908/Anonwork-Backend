using Anonwork.Application.Features.Roles.DTOs.Requests;
using Anonwork.Application.Interfaces;
using Anonwork.Domain.Entities;

namespace Anonwork.Application.Features.Roles;

public class GetRoleByIdUseCase(IUnitOfWork unitOfWork)
{
    private readonly IGenericRepository<Role> _roleRepo = unitOfWork.GetRepository<Role>();

    public async Task<RoleDto> ExecuteAsync(Guid id, CancellationToken ct = default)
    {
        var role = await _roleRepo.GetByIdAsync(id, ct)
            ?? throw new KeyNotFoundException($"Role with id '{id}' not found.");

        return Map(role);
    }

    private static RoleDto Map(Role role) => new(role.Id, role.Name, role.Description, role.IsActive, role.CreatedAt, role.UpdatedAt);
}
