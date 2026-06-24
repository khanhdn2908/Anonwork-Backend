using Anonwork.Application.Common.Exceptions;
using Anonwork.Application.Features.AnonImages.DTOs.Requests;
using Anonwork.Application.Features.AnonImages.DTOs.Responses;
using Anonwork.Application.Interfaces;
using Anonwork.Domain.Entities;

namespace Anonwork.Application.Features.AnonImages;

public class UpdateAnonImageUseCase(IUnitOfWork unitOfWork, IR2Service r2Service)
{
    private readonly IGenericRepository<AnonImage> _anonImageRepo = unitOfWork.GetRepository<AnonImage>();
    private readonly IR2Service _r2Service = r2Service;

    public async Task<AnonImageResponseDto> ExecuteAsync(Guid anonImageId, UpdateAnonImageRequestDto request, CancellationToken ct = default)
    {
        if (anonImageId == Guid.Empty)
            throw new ArgumentException("Anon image id is required.");

        var anonImage = await _anonImageRepo.GetByIdAsync(anonImageId, ct)
            ?? throw new NotFoundException(nameof(AnonImage), anonImageId);

        if(!string.IsNullOrWhiteSpace(request.Name))
            anonImage.Name = request.Name.Trim();

        string? fileKey = null;
        if (request.Image is null)
        {
            fileKey = anonImage.FileKey;
        }
        else 
        {
            var file = await _r2Service.UploadFileAsync(request.Image, "anon-images", ct);
            fileKey = file.FileKey;
        }

        if (!string.IsNullOrWhiteSpace(fileKey))
        {
            anonImage.FileKey = fileKey;
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
            FileKey: anonImage.FileKey,
            IsActive: anonImage.IsActive,
            CreatedAt: anonImage.CreatedAt,
            UpdatedAt: anonImage.UpdatedAt);
}
