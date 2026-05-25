using Anonwork.Application.Features.Subjects.DTOs;
using Anonwork.Application.Interfaces;
using Anonwork.Domain.Common.Exceptions;
using Subject = Anonwork.Domain.Entities.Subject;

namespace Anonwork.Application.Features.Subjects;

/// <summary>
/// Use case for getting a subject by id
/// </summary>
public class GetSubjectByIdUseCase(ISubjectRepository subjectRepo)
{
    public async Task<SubjectResponseDto> ExecuteAsync(Guid subjectId, CancellationToken ct = default)
    {
        // ── Validation ──────────────────────────────
        if (subjectId == Guid.Empty)
            throw new ArgumentException("Subject id is required.");

        // ── Get subject ─────────────────────────────
        var subject = await subjectRepo.GetByIdAsync(subjectId, ct);

        if (subject is null)
            throw new NotFoundException(nameof(Subject), subjectId);

        // ── Return response ─────────────────────────
        return MapToResponse(subject);
    }

    private static SubjectResponseDto MapToResponse(Subject subject)
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
