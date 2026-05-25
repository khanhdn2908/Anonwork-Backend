using Anonwork.Application.Features.Subjects.DTOs;
using Anonwork.Application.Interfaces;
using Anonwork.Domain.Common.Exceptions;
using Subject = Anonwork.Domain.Entities.Subject;

namespace Anonwork.Application.Features.Subjects;

/// <summary>
/// Use case for updating an existing subject
/// </summary>
public class UpdateSubjectUseCase(ISubjectRepository subjectRepo)
{
    public async Task<SubjectResponseDto> ExecuteAsync(
        Guid subjectId,
        UpdateSubjectRequestDto request,
        CancellationToken ct = default)
    {
        // ── Validation ──────────────────────────────
        if (subjectId == Guid.Empty)
            throw new ArgumentException("Subject id is required.");

        if (string.IsNullOrWhiteSpace(request.Name))
            throw new ArgumentException("Subject name is required.");

        if (string.IsNullOrWhiteSpace(request.Slug))
            throw new ArgumentException("Subject slug is required.");

        // ── Get existing subject ────────────────────
        var subject = await subjectRepo.GetByIdAsync(subjectId, ct);

        if (subject is null)
            throw new NotFoundException(nameof(Subject), subjectId);

        // ── Check if new slug already exists ────────
        var normalizedNewSlug = request.Slug.Trim().ToLower();
        if (subject.Slug != normalizedNewSlug)
        {
            var slugExists = await subjectRepo.ExistsBySlugAsync(normalizedNewSlug, ct);
            if (slugExists)
                throw new InvalidOperationException($"Subject with slug '{normalizedNewSlug}' already exists.");
        }

        // ── Update subject ──────────────────────────
        subject.Name = request.Name.Trim();
        subject.Slug = normalizedNewSlug;
        subject.IconEmoji = request.IconEmoji?.Trim();

        // ── Save to repository ──────────────────────
        await subjectRepo.UpdateAsync(subject, ct);

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
