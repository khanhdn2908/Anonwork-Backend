namespace Anonwork.Application.Features.Permissions.DTOs.Requests;

public record PermissionDto(
    Guid Id,
    string Code,
    string? Description,
    bool IsActive,
    DateTime CreatedAt,
    DateTime UpdatedAt
);
