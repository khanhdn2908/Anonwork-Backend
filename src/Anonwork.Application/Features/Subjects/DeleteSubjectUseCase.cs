using Anonwork.Application.Interfaces;
using Anonwork.Domain.Common.Exceptions;
using Subject = Anonwork.Domain.Entities.Subject;

namespace Anonwork.Application.Features.Subjects;

/// <summary>
/// Use case for deleting a subject
/// </summary>
public class DeleteSubjectUseCase(ISubjectRepository subjectRepo)
{
    public async Task ExecuteAsync(Guid subjectId, CancellationToken ct = default)
    {
        // ── Validation ──────────────────────────────
        if (subjectId == Guid.Empty)
            throw new ArgumentException("Subject id is required.");

        // ── Check if subject exists ─────────────────
        var exists = await subjectRepo.ExistsByIdAsync(subjectId, ct);

        if (!exists)
            throw new NotFoundException(nameof(Subject), subjectId);

        // ── Delete subject ──────────────────────────
        await subjectRepo.DeleteAsync(subjectId, ct);
    }
}
