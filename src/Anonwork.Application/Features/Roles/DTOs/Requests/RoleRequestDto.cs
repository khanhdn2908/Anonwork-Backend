namespace Anonwork.Application.Features.Roles.DTOs.Requests;

public record RoleRequestDto(
    string Name,
    string? Description,
    bool IsActive = true
);
