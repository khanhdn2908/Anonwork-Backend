namespace Anonwork.Application.Features.Permissions.DTOs;

public record PermissionDto(
    Guid Id,
    string Code,
    string? Description,
    bool IsActive,
    DateTime CreatedAt,
    DateTime UpdatedAt
);
