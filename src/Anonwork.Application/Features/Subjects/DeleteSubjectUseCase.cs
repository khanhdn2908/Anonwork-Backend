using Anonwork.Application.Common.Exceptions;
using Anonwork.Application.Interfaces;
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

        var subject = await _subjectRepo.GetByIdWithTrackingAsync(subjectId, ct);

        if (subject is null)
            throw new NotFoundException(nameof(Subject), subjectId);

        subject.IsActive = false;
        subject.UpdatedAt = DateTime.UtcNow;
        await unitOfWork.SaveChangesAsync(ct);
    }
}
