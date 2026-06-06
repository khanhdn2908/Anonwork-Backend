namespace Anonwork.Application.Features.Subjects.DTOs.Requests;

/// <summary>
/// DTO for update subject request
/// </summary>
public record UpdateSubjectRequestDto(
    string Name,
    string Slug,
    string? IconEmoji
);
