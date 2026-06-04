namespace Anonwork.Application.Features.Roles.DTOs;

public record RoleRequestDto(
    string Name,
    string? Description,
    bool IsActive = true
);
