namespace Anonwork.Application.Features.AnonImages.DTOs.Requests;

public record CreateAnonImageRequestDto(
    string Name,
    string ImageUrl,
    bool IsActive = true
);
