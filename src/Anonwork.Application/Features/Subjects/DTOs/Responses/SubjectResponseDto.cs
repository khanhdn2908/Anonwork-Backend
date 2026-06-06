namespace Anonwork.Application.Features.Subjects.DTOs.Responses;

/// <summary>
/// DTO for subject response
/// </summary>
public record SubjectResponseDto(
    Guid Id,
    string Name,
    string Slug,
    string? IconEmoji,
    int PostCount,
    DateTime CreatedAt
);
