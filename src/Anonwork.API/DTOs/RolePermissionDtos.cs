namespace Anonwork.API.DTOs;

public record RoleDto(
    Guid Id,
    string Name,
    string? Description,
    bool IsActive,
    DateTime CreatedAt,
    DateTime UpdatedAt
);

public record RoleRequestDto(
    string Name,
    string? Description,
    bool IsActive = true
);

public record PermissionDto(
    Guid Id,
    string Code,
    string? Description,
    bool IsActive,
    DateTime CreatedAt,
    DateTime UpdatedAt
);

public record PermissionRequestDto(
    string Code,
    string? Description,
    bool IsActive = true
);
