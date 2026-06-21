using Anonwork.Application.Common.Exceptions;
using Anonwork.Application.Features.AnonImages.DTOs.Responses;
using Anonwork.Application.Interfaces;
using Anonwork.Domain.Entities;
using AutoMapper.Execution;

namespace Anonwork.Application.Features.AnonImages;

public class GetAnonImageByIdUseCase(IUnitOfWork unitOfWork)
{
    private readonly IGenericRepository<AnonImage> _anonImageRepo = unitOfWork.GetRepository<AnonImage>();

    public async Task<AnonImageResponseDto> ExecuteAsync(Guid anonImageId, bool hasPermision, CancellationToken ct = default)
    {
        if (anonImageId == Guid.Empty)
            throw new ArgumentException("Anon image id is required.");

        var anonImage = await _anonImageRepo.GetByIdAsync(anonImageId, ct)
            ?? throw new NotFoundException(nameof(AnonImage), anonImageId);

        if (!hasPermision && !anonImage.IsActive)
            throw new NotFoundException(nameof(AnonImage), anonImageId);

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
