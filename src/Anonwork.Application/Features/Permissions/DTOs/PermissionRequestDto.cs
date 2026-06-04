namespace Anonwork.Application.Features.Permissions.DTOs;

public record PermissionRequestDto(
    string Code,
    string? Description,
    bool IsActive = true
);
