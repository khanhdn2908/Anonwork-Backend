using Anonwork.Application.Common;
using Anonwork.Application.Common.Exceptions;
using Anonwork.Application.Interfaces;
using Anonwork.Domain.Entities;

namespace Anonwork.Application.Features.Users;

public class AssignAnonImageToUserUseCase(IUnitOfWork unitOfWork, IPlanAccessService planAccessService, IActivityLogService activityLogService)
{
    private readonly IGenericRepository<User> _userRepo = unitOfWork.GetRepository<User>();
    private readonly IGenericRepository<AnonImage> _anonImageRepo = unitOfWork.GetRepository<AnonImage>();
    private readonly IPlanAccessService _planAccessService = planAccessService;
    private readonly IActivityLogService _activityLogService = activityLogService;

    public async Task ExecuteAsync(
        Guid userId,
        Guid anonImageId,
        CancellationToken ct = default)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("User id is required.");

        if (anonImageId == Guid.Empty)
            throw new ArgumentException("Anon image id is required.");

        var user = await _userRepo.GetByIdAsync(userId, ct)
            ?? throw new NotFoundException(nameof(User), userId);

        var anonImage = await _anonImageRepo.GetByIdAsync(anonImageId, ct)
            ?? throw new NotFoundException(nameof(AnonImage), anonImageId);

        await _planAccessService.EnsureCanUseAnonImageAsync(userId, anonImage, ct);

        if (!anonImage.IsActive)
            throw new InvalidOperationException("Anon image is inactive.");

        var anonAlias = AnonAliasGenerator.GenerateFromImageName(anonImage.Name);
        user.SetAnonInfo(anonImageId, anonAlias);

        await _userRepo.UpdateAsync(user, ct);
        await unitOfWork.SaveChangesAsync(ct);

        _ = _activityLogService.LogAsync(
            userId,
            "CHANGE_ANON_IMAGE",
            "User",
            $"Thay đổi ảnh ẩn danh thành '{anonImage.Name}' (Alias: {anonAlias})",
            "anon_image",
            anonImageId.ToString(),
            ct: ct);
    }
}
