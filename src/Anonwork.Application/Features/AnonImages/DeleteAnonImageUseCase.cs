using Anonwork.Application.Common.Exceptions;
using Anonwork.Application.Interfaces;
using Anonwork.Domain.Entities;

namespace Anonwork.Application.Features.AnonImages;

public class DeleteAnonImageUseCase(IUnitOfWork unitOfWork)
{
    private readonly IGenericRepository<AnonImage> _anonImageRepo = unitOfWork.GetRepository<AnonImage>();

    public async Task ExecuteAsync(Guid anonImageId, CancellationToken ct = default)
    {
        if (anonImageId == Guid.Empty)
            throw new ArgumentException("Anon image id is required.");

        var anonImage = await _anonImageRepo.GetByIdWithTrackingAsync(anonImageId, ct)
            ?? throw new NotFoundException(nameof(AnonImage), anonImageId);

        if (anonImage.IsActive == false) throw new ArgumentException("Anon image is deleted");

        anonImage.IsActive = false;
        anonImage.UpdatedAt = DateTime.UtcNow;

        await unitOfWork.SaveChangesAsync(ct);
    }
}
