using Anonwork.Application.Features.AnonImages.DTOs.Requests;
using Anonwork.Application.Features.AnonImages.DTOs.Responses;
using Anonwork.Application.Interfaces;
using Anonwork.Domain.Entities;

namespace Anonwork.Application.Features.AnonImages;

public class CreateAnonImageUseCase(IUnitOfWork unitOfWork, IR2Service r2Service)
{
    private readonly IGenericRepository<AnonImage> _anonImageRepo = unitOfWork.GetRepository<AnonImage>();
    private readonly IR2Service _r2Service = r2Service;

    public async Task<AnonImageResponseDto> ExecuteAsync(CreateAnonImageRequestDto request, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
            throw new ArgumentException("Anon image name is required.");

        if (request.Image is null || request.Image.Length == 0)
            throw new ArgumentException("Anon image url is required.");

        var file = await _r2Service.UploadFileAsync(request.Image, "anon-images", ct);

        if(string.IsNullOrWhiteSpace(file.FileUrl))
            throw new ArgumentException("Upload image is fail.");

        var entity = new AnonImage
        {
            Id = Guid.NewGuid(),
            Name = request.Name.Trim(),
            FileKey = file.FileKey,
            IsActive = request.IsActive,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var created = await _anonImageRepo.AddAsync(entity, ct);
        await unitOfWork.SaveChangesAsync(ct);

        return MapToResponse(created, file.FileUrl);
    }

    private static AnonImageResponseDto MapToResponse(AnonImage anonImage, string fileUrl)
        => new(
            Id: anonImage.Id,
            Name: anonImage.Name,
            FileKey: anonImage.FileKey,
            FileUrl: fileUrl,
            IsActive: anonImage.IsActive,
            CreatedAt: anonImage.CreatedAt,
            UpdatedAt: anonImage.UpdatedAt
        );
}
