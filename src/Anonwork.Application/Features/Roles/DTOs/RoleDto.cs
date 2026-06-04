namespace Anonwork.Application.Features.Roles.DTOs;

public record RoleDto(
    Guid Id,
    string Name,
    string? Description,
    bool IsActive,
    DateTime CreatedAt,
    DateTime UpdatedAt
);
