namespace Anonwork.Application.Features.AnonImages.DTOs.Requests;

public record UpdateAnonImageRequestDto(
    string Name,
    string? ImageUrl,
    bool IsActive
);
