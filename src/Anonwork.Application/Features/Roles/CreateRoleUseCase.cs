using Anonwork.Application.Features.Roles.DTOs;
using Anonwork.Application.Interfaces;
using Anonwork.Domain.Entities;

namespace Anonwork.Application.Features.Roles;

public class CreateRoleUseCase(IUnitOfWork unitOfWork)
{
    private readonly IGenericRepository<Role> _roleRepo = unitOfWork.GetRepository<Role>();

    public async Task<RoleDto> ExecuteAsync(RoleRequestDto request, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
            throw new ArgumentException("Role name is required.");

        var name = request.Name.Trim().ToLowerInvariant();
        var existing = await _roleRepo.FindSingleAsync(r => r.Name == name, ct);
        if (existing is not null)
            throw new InvalidOperationException($"Role with name '{request.Name}' already exists.");

        var role = Role.Create(name, request.Description);
        role.IsActive = request.IsActive;

        var created = await _roleRepo.AddAsync(role, ct);
        await unitOfWork.SaveChangesAsync(ct);

        return Map(created);
    }

    private static RoleDto Map(Role role) => new(role.Id, role.Name, role.Description, role.IsActive, role.CreatedAt, role.UpdatedAt);
}
