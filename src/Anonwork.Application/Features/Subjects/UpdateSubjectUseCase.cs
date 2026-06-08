using Anonwork.Application.Features.Subjects.DTOs.Requests;
using Anonwork.Application.Features.Subjects.DTOs.Responses;
using Anonwork.Application.Interfaces;
using Anonwork.Domain.Common.Exceptions;
using Subject = Anonwork.Domain.Entities.Subject;

namespace Anonwork.Application.Features.Subjects;

/// <summary>
/// Use case for updating an existing subject
/// </summary>
public class UpdateSubjectUseCase(IUnitOfWork unitOfWork)
{
    private readonly IGenericRepository<Subject> _subjectRepo = unitOfWork.GetRepository<Subject>();

    public async Task<SubjectResponseDto> ExecuteAsync(
        Guid subjectId,
        UpdateSubjectRequestDto request,
        CancellationToken ct = default)
    {
        if (subjectId == Guid.Empty)
            throw new ArgumentException("Subject id is required.");

        if (string.IsNullOrWhiteSpace(request.Name))
            throw new ArgumentException("Subject name is required.");

        if (string.IsNullOrWhiteSpace(request.Slug))
            throw new ArgumentException("Subject slug is required.");

        var subject = await _subjectRepo.GetByIdAsync(subjectId, ct)
            ?? throw new NotFoundException(nameof(Subject), subjectId);

        var normalizedNewSlug = request.Slug.Trim().ToLower();
        if (subject.Slug != normalizedNewSlug)
        {
            var slugExists = await _subjectRepo.FindSingleAsync(s => s.Slug == normalizedNewSlug, ct);
            if (slugExists is not null)
                throw new InvalidOperationException($"Subject with slug '{normalizedNewSlug}' already exists.");
        }

        subject.Name = request.Name.Trim();
        subject.Slug = normalizedNewSlug;
        subject.IconEmoji = request.IconEmoji?.Trim();

        subject.UpdatedAt = DateTime.UtcNow;

        await _subjectRepo.UpdateAsync(subject, ct);
        await unitOfWork.SaveChangesAsync(ct);

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
