namespace Anonwork.Application.Features.AnonImages.DTOs.Responses;

public record AnonImageResponseDto(
    Guid Id,
    string Name,
    string FileKey,
    bool IsActive,
    DateTime CreatedAt,
    DateTime UpdatedAt
);
