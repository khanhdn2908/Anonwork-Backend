using Anonwork.Application.Features.AnonImages.DTOs.Requests;
using Anonwork.Application.Features.AnonImages.DTOs.Responses;
using Anonwork.Application.Interfaces;
using Anonwork.Domain.Entities;

namespace Anonwork.Application.Features.AnonImages;

public class CreateAnonImageUseCase(IUnitOfWork unitOfWork)
{
    private readonly IGenericRepository<AnonImage> _anonImageRepo = unitOfWork.GetRepository<AnonImage>();

    public async Task<AnonImageResponseDto> ExecuteAsync(CreateAnonImageRequestDto request, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
            throw new ArgumentException("Anon image name is required.");

        if (string.IsNullOrWhiteSpace(request.ImageUrl))
            throw new ArgumentException("Anon image url is required.");

        var entity = new AnonImage
        {
            Id = Guid.NewGuid(),
            Name = request.Name.Trim(),
            ImageUrl = request.ImageUrl.Trim(),
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
