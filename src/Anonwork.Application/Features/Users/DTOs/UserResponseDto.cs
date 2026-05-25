namespace Anonwork.Application.Features.Users.DTOs;

public record UserResponseDto(
    Guid Id,
    string Username,
    string Email,
    string? AvatarUrl,
    string? Bio,
    string AnonAlias,
    bool IsAnonDefault,
    string Role,
    DateTime CreatedAt,
    DateTime UpdatedAt
);
