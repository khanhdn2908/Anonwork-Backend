using Anonwork.Application.Features.AnonImages.DTOs.Responses;
using Anonwork.Application.Interfaces;
using Anonwork.Domain.Entities;

namespace Anonwork.Application.Features.AnonImages;

public class GetAllAnonImagesUseCase(IUnitOfWork unitOfWork)
{
    private readonly IGenericRepository<AnonImage> _anonImageRepo = unitOfWork.GetRepository<AnonImage>();

    public async Task<IReadOnlyList<AnonImageResponseDto>> ExecuteAsync(
        bool? isActive = null,
        CancellationToken ct = default)
    {
        var items = await _anonImageRepo.GetAllAsync(ct);

        if (isActive.HasValue)
        {
            items = items.Where(x => x.IsActive == isActive.Value).ToList();
        }

        return items
            .OrderByDescending(x => x.CreatedAt)
            .Select(MapToResponse)
            .ToList();
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
