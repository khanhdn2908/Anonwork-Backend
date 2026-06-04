using Anonwork.Application.Features.Roles.DTOs;
using Anonwork.Application.Interfaces;
using Anonwork.Domain.Entities;

namespace Anonwork.Application.Features.Roles;

public class UpdateRoleUseCase(IUnitOfWork unitOfWork)
{
    private readonly IGenericRepository<Role> _roleRepo = unitOfWork.GetRepository<Role>();

    public async Task<RoleDto> ExecuteAsync(Guid id, RoleRequestDto request, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
            throw new ArgumentException("Role name is required.");

        var role = await _roleRepo.GetByIdWithTrackingAsync(id, ct)
            ?? throw new KeyNotFoundException($"Role with id '{id}' not found.");

        var name = request.Name.Trim().ToLowerInvariant();
        var duplicate = await _roleRepo.FindSingleAsync(r => r.Name == name && r.Id != id, ct);
        if (duplicate is not null)
            throw new InvalidOperationException($"Role with name '{request.Name}' already exists.");

        role.Name = name;
        role.Description = request.Description;
        role.IsActive = request.IsActive;
        role.UpdatedAt = DateTime.UtcNow;

        await _roleRepo.UpdateAsync(role, ct);
        await unitOfWork.SaveChangesAsync(ct);

        return Map(role);
    }

    private static RoleDto Map(Role role) => new(role.Id, role.Name, role.Description, role.IsActive, role.CreatedAt, role.UpdatedAt);
}
