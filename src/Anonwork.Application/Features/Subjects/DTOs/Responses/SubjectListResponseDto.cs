namespace Anonwork.Application.Features.Subjects.DTOs.Responses;

/// <summary>
/// DTO for subject list response with pagination
/// </summary>
public record SubjectListResponseDto(
    List<SubjectResponseDto> Subjects,
    int Total,
    int Page,
    int PageSize,
    int TotalPages
);
