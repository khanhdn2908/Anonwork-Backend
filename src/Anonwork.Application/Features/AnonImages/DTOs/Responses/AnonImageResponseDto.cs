namespace Anonwork.Application.Features.AnonImages.DTOs.Responses;

public record AnonImageResponseDto(
    Guid Id,
    string Name,
    string ImageUrl,
    bool IsActive,
    DateTime CreatedAt,
    DateTime UpdatedAt
);
