namespace Anonwork.Application.Features.Subjects.DTOs;

/// <summary>
/// DTO for create subject request
/// </summary>
public record CreateSubjectRequestDto(
    string Name,
    string Slug,
    string? IconEmoji
);
