using Anonwork.Application.Interfaces;
using Anonwork.Domain.Common.Exceptions;
using Anonwork.Domain.Entities;

namespace Anonwork.Application.Features.AnonImages;

public class DeleteAnonImageUseCase(IUnitOfWork unitOfWork)
{
    private readonly IGenericRepository<AnonImage> _anonImageRepo = unitOfWork.GetRepository<AnonImage>();

    public async Task ExecuteAsync(Guid anonImageId, CancellationToken ct = default)
    {
        if (anonImageId == Guid.Empty)
            throw new ArgumentException("Anon image id is required.");

        var anonImage = await _anonImageRepo.GetByIdAsync(anonImageId, ct)
            ?? throw new NotFoundException(nameof(AnonImage), anonImageId);

        await _anonImageRepo.DeleteAsync(anonImage, ct);
        await unitOfWork.SaveChangesAsync(ct);
    }
}
