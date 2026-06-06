namespace Anonwork.Application.Features.Permissions.DTOs.Requests;

public record PermissionRequestDto(
    string Code,
    string? Description,
    bool IsActive = true
);
