using Anonwork.Application.Features.AnonImages.DTOs.Requests;
using Anonwork.Application.Features.AnonImages.DTOs.Responses;
using Anonwork.Application.Interfaces;
using Anonwork.Domain.Common.Exceptions;
using Anonwork.Domain.Entities;

namespace Anonwork.Application.Features.AnonImages;

public class UpdateAnonImageUseCase(IUnitOfWork unitOfWork)
{
    private readonly IGenericRepository<AnonImage> _anonImageRepo = unitOfWork.GetRepository<AnonImage>();

    public async Task<AnonImageResponseDto> ExecuteAsync(Guid anonImageId, UpdateAnonImageRequestDto request, CancellationToken ct = default)
    {
        if (anonImageId == Guid.Empty)
            throw new ArgumentException("Anon image id is required.");

        if (string.IsNullOrWhiteSpace(request.Name))
            throw new ArgumentException("Anon image name is required.");

        var anonImage = await _anonImageRepo.GetByIdAsync(anonImageId, ct)
            ?? throw new NotFoundException(nameof(AnonImage), anonImageId);

        anonImage.Name = request.Name.Trim();
        if (!string.IsNullOrWhiteSpace(request.ImageUrl))
        {
            anonImage.ImageUrl = request.ImageUrl.Trim();
        }
        anonImage.IsActive = request.IsActive;
        anonImage.UpdatedAt = DateTime.UtcNow;

        await _anonImageRepo.UpdateAsync(anonImage, ct);
        await unitOfWork.SaveChangesAsync(ct);

        return MapToResponse(anonImage);
    }

    private static AnonImageResponseDto MapToResponse(AnonImage anonImage)
        => new(
            Id: anonImage.Id,
            Name: anonImage.Name,
            ImageUrl: anonImage.ImageUrl,
            IsActive: anonImage.IsActive,
            CreatedAt: anonImage.CreatedAt,
            UpdatedAt: anonImage.UpdatedAt);
}
