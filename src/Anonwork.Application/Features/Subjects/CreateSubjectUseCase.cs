using Anonwork.Application.Features.Subjects.DTOs;
using Anonwork.Application.Interfaces;
using Anonwork.Domain.Entities;

namespace Anonwork.Application.Features.Subjects;

/// <summary>
/// Use case for creating a new subject
/// </summary>
public class CreateSubjectUseCase(ISubjectRepository subjectRepo)
{
    public async Task<SubjectResponseDto> ExecuteAsync(
        CreateSubjectRequestDto request,
        CancellationToken ct = default)
    {
        // ── Validation ──────────────────────────────
        if (string.IsNullOrWhiteSpace(request.Name))
            throw new ArgumentException("Subject name is required.");

        if (string.IsNullOrWhiteSpace(request.Slug))
            throw new ArgumentException("Subject slug is required.");

        // ── Check if slug already exists ─────────────
        var slugExists = await subjectRepo.ExistsBySlugAsync(request.Slug, ct);
        if (slugExists)
            throw new InvalidOperationException($"Subject with slug '{request.Slug}' already exists.");

        // ── Create subject ──────────────────────────
        var subject = new Subject
        {
            Id = Guid.NewGuid(),
            Name = request.Name.Trim(),
            Slug = request.Slug.Trim().ToLower(),
            IconEmoji = request.IconEmoji?.Trim(),
            PostCount = 0,
            CreatedAt = DateTime.UtcNow
        };

        // ── Save to repository ──────────────────────
        var createdSubject = await subjectRepo.CreateAsync(subject, ct);

        // ── Return response ─────────────────────────
        return MapToResponse(createdSubject);
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
