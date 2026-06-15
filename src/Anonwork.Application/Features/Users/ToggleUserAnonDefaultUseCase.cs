using Anonwork.Application.Common.Exceptions;
using Anonwork.Application.Interfaces;
using Anonwork.Domain.Entities;

namespace Anonwork.Application.Features.Users;

public class ToggleUserAnonDefaultUseCase(IUnitOfWork unitOfWork)
{
    private readonly IGenericRepository<User> _userRepo = unitOfWork.GetRepository<User>();

    public async Task ExecuteAsync(
        Guid userId,
        CancellationToken ct = default)
    {
        var user = await _userRepo.GetByIdAsync(userId, ct)
            ?? throw new NotFoundException("User not found.");

        if (!user.IsAnonDefault)
            user.EnableAnonDefault();
        else
            user.DisableAnonDefault();

        await _userRepo.UpdateAsync(user, ct);
        await unitOfWork.SaveChangesAsync(ct);
    }
}
