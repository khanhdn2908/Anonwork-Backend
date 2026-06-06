namespace Anonwork.Application.Features.Users.DTOs.Responses;

public record UserListResponseDto(
    Guid Id,
    string Username,
    string? AvatarUrl,
    string? Bio,
    string AnonAlias,
    DateTime CreatedAt
);

public record UserListPaginatedResponseDto(
    List<UserListResponseDto> Users,
    int Total,
    int Page,
    int PageSize,
    int TotalPages
);
