namespace Anonwork.Application.Features.Roles.DTOs;

public record AssignPermissionsRequestDto(IReadOnlyCollection<Guid> PermissionIds);
