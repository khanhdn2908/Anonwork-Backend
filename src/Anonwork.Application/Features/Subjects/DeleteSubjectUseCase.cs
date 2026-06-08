using Anonwork.Application.Interfaces;
using Anonwork.Domain.Common.Exceptions;
using Anonwork.Domain.Entities;

namespace Anonwork.Application.Features.Subjects;

/// <summary>
/// Use case for deleting a subject
/// </summary>
public class DeleteSubjectUseCase(IUnitOfWork unitOfWork)
{
    private readonly IGenericRepository<Subject> _subjectRepo = unitOfWork.GetRepository<Subject>();

    public async Task ExecuteAsync(Guid subjectId, CancellationToken ct = default)
    {
        if (subjectId == Guid.Empty)
            throw new ArgumentException("Subject id is required.");

        var subject = await _subjectRepo.GetByIdAsync(subjectId, ct);

        if (subject is null)
            throw new NotFoundException(nameof(Subject), subjectId);

        await _subjectRepo.DeleteAsync(subjectId, ct);
        await unitOfWork.SaveChangesAsync(ct);
    }
}
