namespace Anonwork.Application.Features.Roles.DTOs.Requests;

public record AssignPermissionsRequestDto(IReadOnlyCollection<Guid> PermissionIds);
