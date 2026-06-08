using Anonwork.Application.Features.Subjects.DTOs.Requests;
using Anonwork.Application.Features.Subjects.DTOs.Responses;
using Anonwork.Application.Interfaces;
using Anonwork.Domain.Entities;

namespace Anonwork.Application.Features.Subjects;

/// <summary>
/// Use case for creating a new subject
/// </summary>
public class CreateSubjectUseCase(IUnitOfWork unitOfWork)
{
    private readonly IGenericRepository<Subject> _subjectRepo = unitOfWork.GetRepository<Subject>();

    public async Task<SubjectResponseDto> ExecuteAsync(
        CreateSubjectRequestDto request,
        CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
            throw new ArgumentException("Subject name is required.");

        if (string.IsNullOrWhiteSpace(request.Slug))
            throw new ArgumentException("Subject slug is required.");

        var existing = await _subjectRepo.FindSingleAsync(s => s.Slug == request.Slug.Trim().ToLower(), ct);
        if (existing is not null)
            throw new InvalidOperationException($"Subject with slug '{request.Slug}' already exists.");

        var subject = new Subject
        {
            Id = Guid.NewGuid(),
            Name = request.Name.Trim(),
            Slug = request.Slug.Trim().ToLower(),
            IconEmoji = request.IconEmoji?.Trim(),
            PostCount = 0,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        };

        var created = await _subjectRepo.AddAsync(subject, ct);
        await unitOfWork.SaveChangesAsync(ct);

        return MapToResponse(created);
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
