namespace Anonwork.Application.Features.Users.DTOs.Responses;

public record GetMeResponseDto(
    Guid Id,
    string Username,
    string Email,
    string? AvatarUrl,
    string? Bio,
    string AnonAlias,
    bool IsAnonDefault,
    DateTime CreatedAt,
    DateTime UpdatedAt
);
