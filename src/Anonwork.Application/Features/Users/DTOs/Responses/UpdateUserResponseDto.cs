namespace Anonwork.Application.Features.Users.DTOs.Responses;

public record UpdateUserResponseDto(
    string Username,
    string? Bio,
    string? AvatarKey,
    string? AvatarUrl
);
