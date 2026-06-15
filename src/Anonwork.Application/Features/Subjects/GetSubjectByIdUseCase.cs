using Anonwork.Application.Common.Exceptions;
using Anonwork.Application.Features.Subjects.DTOs.Responses;
using Anonwork.Application.Interfaces;
using Subject = Anonwork.Domain.Entities.Subject;

namespace Anonwork.Application.Features.Subjects;

/// <summary>
/// Use case for getting a subject by id
/// </summary>
public class GetSubjectByIdUseCase(IUnitOfWork unitOfWork)
{
    private readonly IGenericRepository<Subject> _subjectRepo = unitOfWork.GetRepository<Subject>();

    public async Task<SubjectResponseDto> ExecuteAsync(Guid subjectId, CancellationToken ct = default)
    {
        // ── Validation ──────────────────────────────
        if (subjectId == Guid.Empty)
            throw new ArgumentException("Subject id is required.");

        // ── Get subject ─────────────────────────────
        var subject = await _subjectRepo.GetByIdAsync(subjectId, ct);

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
            IsActive: subject.IsActive,
            CreatedAt: subject.CreatedAt,
            UpdatedAt: subject.UpdatedAt
        );
    }
}
