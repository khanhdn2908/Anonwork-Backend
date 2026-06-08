using Anonwork.Application.Features.AnonImages.DTOs.Requests;
using Anonwork.Application.Features.AnonImages.DTOs.Responses;
using Anonwork.Application.Interfaces;
using Anonwork.Domain.Entities;

namespace Anonwork.Application.Features.AnonImages;

public class CreateAnonImageUseCase(IUnitOfWork unitOfWork, ICloudinaryService cloudinaryService)
{
    private readonly IGenericRepository<AnonImage> _anonImageRepo = unitOfWork.GetRepository<AnonImage>();
    private readonly ICloudinaryService _cloudinaryService = cloudinaryService;

    public async Task<AnonImageResponseDto> ExecuteAsync(CreateAnonImageRequestDto request, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
            throw new ArgumentException("Anon image name is required.");

        if (request.Image is null || request.Image.Length == 0)
            throw new ArgumentException("Anon image url is required.");

        var imageUrl = await _cloudinaryService.UploadImageAsync(request.Image, "anon-images", ct);

        if(string.IsNullOrWhiteSpace(imageUrl))
            throw new ArgumentException("Upload image is fail.");

        var entity = new AnonImage
        {
            Id = Guid.NewGuid(),
            Name = request.Name.Trim(),
            ImageUrl = imageUrl,
            IsActive = request.IsActive,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var created = await _anonImageRepo.AddAsync(entity, ct);
        await unitOfWork.SaveChangesAsync(ct);

        return MapToResponse(created);
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
