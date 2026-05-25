namespace Anonwork.Application.Features.Users.DTOs;

public record UserListResponseDto(
    Guid Id,
    string Username,
    string? AvatarUrl,
    string? Bio,
    string AnonAlias,
    string Role,
    DateTime CreatedAt
);

public record UserListPaginatedResponseDto(
    List<UserListResponseDto> Users,
    int Total,
    int Page,
    int PageSize,
    int TotalPages
);
