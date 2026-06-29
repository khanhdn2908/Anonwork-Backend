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

        if (!string.IsNullOrWhiteSpace(request.Name))
            anonImage.Name = request.Name.Trim();

        string? oldFileKey = null;
        string? fileUrl = null;

        if (request.Image is not null)
        {
            oldFileKey = anonImage.FileKey;

            var uploadedFile = await _r2Service.UploadFileAsync(request.Image, "anon-images", ct);
            anonImage.FileKey = uploadedFile.FileKey;
            fileUrl = uploadedFile.FileUrl;
        }
        else
        {
            fileUrl = _r2Service.GetPublicUrl(anonImage.FileKey);
        }

        anonImage.IsActive = request.IsActive;
        anonImage.IsExclusive = request.IsExclusive;
        anonImage.UpdatedAt = DateTime.UtcNow;

        await _anonImageRepo.UpdateAsync(anonImage, ct);
        await unitOfWork.SaveChangesAsync(ct);

        if (!string.IsNullOrWhiteSpace(oldFileKey) && oldFileKey != anonImage.FileKey)
            await _r2Service.DeleteFileAsync(oldFileKey, ct);

        return MapToResponse(anonImage, fileUrl!);
    }

    private static AnonImageResponseDto MapToResponse(AnonImage anonImage, string fileUrl)
        => new(
            Id: anonImage.Id,
            Name: anonImage.Name,
            FileKey: anonImage.FileKey,
            FileUrl: fileUrl,
            IsActive: anonImage.IsActive,
            IsExclusive: anonImage.IsExclusive,
            CreatedAt: anonImage.CreatedAt,
            UpdatedAt: anonImage.UpdatedAt);
}
