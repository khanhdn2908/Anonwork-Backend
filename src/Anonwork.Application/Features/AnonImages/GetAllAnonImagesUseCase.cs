using Anonwork.Application.Features.AnonImages.DTOs.Responses;
using Anonwork.Application.Interfaces;
using Anonwork.Domain.Entities;

namespace Anonwork.Application.Features.AnonImages;

public class GetAllAnonImagesUseCase(IUnitOfWork unitOfWork)
{
    private readonly IGenericRepository<AnonImage> _anonImageRepo = unitOfWork.GetRepository<AnonImage>();

    public async Task<IReadOnlyList<AnonImageResponseDto>> ExecuteAsync(
        bool hasPermision,
        CancellationToken ct = default)
    {
        var items = await _anonImageRepo.GetAllAsync(ct);

        if (!hasPermision)
        {
            items = items.Where(x => x.IsActive == true).ToList();
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
            FileKey: anonImage.FileKey,
            IsActive: anonImage.IsActive,
            CreatedAt: anonImage.CreatedAt,
            UpdatedAt: anonImage.UpdatedAt);
}
