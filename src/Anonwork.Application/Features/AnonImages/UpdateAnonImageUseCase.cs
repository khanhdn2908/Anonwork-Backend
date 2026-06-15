using Anonwork.Application.Common.Exceptions;
using Anonwork.Application.Features.AnonImages.DTOs.Requests;
using Anonwork.Application.Features.AnonImages.DTOs.Responses;
using Anonwork.Application.Interfaces;
using Anonwork.Domain.Entities;

namespace Anonwork.Application.Features.AnonImages;

public class UpdateAnonImageUseCase(IUnitOfWork unitOfWork, ICloudinaryService cloudinaryService)
{
    private readonly IGenericRepository<AnonImage> _anonImageRepo = unitOfWork.GetRepository<AnonImage>();
    private readonly ICloudinaryService _cloudinaryService = cloudinaryService;

    public async Task<AnonImageResponseDto> ExecuteAsync(Guid anonImageId, UpdateAnonImageRequestDto request, CancellationToken ct = default)
    {
        if (anonImageId == Guid.Empty)
            throw new ArgumentException("Anon image id is required.");

        var anonImage = await _anonImageRepo.GetByIdAsync(anonImageId, ct)
            ?? throw new NotFoundException(nameof(AnonImage), anonImageId);

        if(!string.IsNullOrWhiteSpace(request.Name))
            anonImage.Name = request.Name.Trim();

        string? imageUrl = null;
        if (!(request.Image is null || request.Image.Length == 0))
        {
            imageUrl = await _cloudinaryService.UploadImageAsync(request.Image, "anon-images", ct);
        }

        if (!string.IsNullOrWhiteSpace(imageUrl))
        {
            anonImage.ImageUrl = imageUrl.Trim();
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
