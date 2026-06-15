using Anonwork.Application.Common.Exceptions;
using Anonwork.Application.Interfaces;
using Anonwork.Domain.Entities;

namespace Anonwork.Application.Features.Subjects;

public class DeleteSubjectUseCasePermanent(IUnitOfWork unitOfWork)
{
    private readonly IGenericRepository<Subject> _subjectRepo = unitOfWork.GetRepository<Subject>();

    public async Task ExecuteAsync(Guid subjectId, CancellationToken ct = default)
    {
        if (subjectId == Guid.Empty)
            throw new ArgumentException("Subject id is required.");

        var subject = await _subjectRepo.GetByIdAsync(subjectId, ct)
            ?? throw new NotFoundException(nameof(Subject), subjectId);

        if (subject.IsActive)
            throw new ArgumentException("Subject need deleted first.");

        await _subjectRepo.DeleteAsync(subjectId, ct);
        await unitOfWork.SaveChangesAsync(ct);
    }
}
