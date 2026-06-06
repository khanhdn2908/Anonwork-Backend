using Anonwork.Application.Features.Subjects.DTOs.Responses;
using Anonwork.Application.Interfaces;
using Anonwork.Domain.Entities;

namespace Anonwork.Application.Features.Subjects;

/// <summary>
/// Use case for getting subjects with search and pagination
/// </summary>
public class GetSubjectsUseCase(IUnitOfWork unitOfWork)
{
    private readonly IGenericRepository<Subject> _subjectRepo = unitOfWork.GetRepository<Subject>();

    public async Task<SubjectListResponseDto> ExecuteAsync(
        string? searchQuery = null,
        int page = 1,
        int pageSize = 10,
        CancellationToken ct = default)
    {
        if (page < 1)
            page = 1;

        if (pageSize < 1 || pageSize > 100)
            pageSize = 10;

        var allSubjects = await _subjectRepo.GetAllAsync(ct);

        if (!string.IsNullOrWhiteSpace(searchQuery))
        {
            var searchTerm = searchQuery.Trim().ToLower();
            allSubjects = allSubjects.Where(s =>
                s.Name.ToLower().Contains(searchTerm) ||
                s.Slug.ToLower().Contains(searchTerm));
        }

        var total = allSubjects.Count();
        var pagedSubjects = allSubjects
            .OrderByDescending(s => s.PostCount)
            .ThenByDescending(s => s.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        var totalPages = (int)Math.Ceiling(total / (double)pageSize);
        var subjectDtos = pagedSubjects.Select(MapToResponse).ToList();

        return new SubjectListResponseDto(
            Subjects: subjectDtos,
            Total: total,
            Page: page,
            PageSize: pageSize,
            TotalPages: totalPages
        );
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
