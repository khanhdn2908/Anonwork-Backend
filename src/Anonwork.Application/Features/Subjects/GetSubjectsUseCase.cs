using Anonwork.Application.Features.Subjects.DTOs;
using Anonwork.Application.Interfaces;

namespace Anonwork.Application.Features.Subjects;

/// <summary>
/// Use case for getting subjects with search and pagination
/// </summary>
public class GetSubjectsUseCase(ISubjectRepository subjectRepo)
{
    public async Task<SubjectListResponseDto> ExecuteAsync(
        string? searchQuery = null,
        int page = 1,
        int pageSize = 10,
        CancellationToken ct = default)
    {
        // ── Validation ──────────────────────────────
        if (page < 1)
            page = 1;

        if (pageSize < 1 || pageSize > 100)
            pageSize = 10;

        // ── Get subjects ────────────────────────────
        var (subjects, total) = await subjectRepo.GetAllAsync(
            searchQuery,
            page,
            pageSize,
            ct);

        // ── Calculate total pages ───────────────────
        var totalPages = (int)Math.Ceiling(total / (double)pageSize);

        // ── Map to response ─────────────────────────
        var subjectDtos = subjects
            .Select(MapToResponse)
            .ToList();

        return new SubjectListResponseDto(
            Subjects: subjectDtos,
            Total: total,
            Page: page,
            PageSize: pageSize,
            TotalPages: totalPages
        );
    }

    private static SubjectResponseDto MapToResponse(Anonwork.Domain.Entities.Subject subject)
    {
        return new SubjectResponseDto(
            Id: subject.Id,
            Name: subject.Name,
            Slug: subject.Slug,
            IconEmoji: subject.IconEmoji,
            PostCount: subject.PostCount,
            CreatedAt: subject.CreatedAt
        );
    }
}
