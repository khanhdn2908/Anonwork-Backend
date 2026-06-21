using Anonwork.Application.Common.Exceptions;
using Anonwork.Application.Interfaces;
using Anonwork.Domain.Entities;

namespace Anonwork.Application.Features.Users;

public class AssignAnonImageToUserUseCase(IUnitOfWork unitOfWork)
{
    private readonly IGenericRepository<User> _userRepo = unitOfWork.GetRepository<User>();
    private readonly IGenericRepository<AnonImage> _anonImageRepo = unitOfWork.GetRepository<AnonImage>();

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

        if (!anonImage.IsActive)
            throw new InvalidOperationException("Anon image is inactive.");

        user.SetAnonInfo(anonImageId, anonImage.Name);

        await _userRepo.UpdateAsync(user, ct);
        await unitOfWork.SaveChangesAsync(ct);
    }
}
