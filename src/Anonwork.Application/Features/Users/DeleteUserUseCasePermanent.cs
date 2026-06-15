using Anonwork.Application.Common.Exceptions;
using Anonwork.Application.Interfaces;
using Anonwork.Domain.Entities;
using Anonwork.Domain.Enums;

namespace Anonwork.Application.Features.Users;

public class DeleteUserUseCasePermanent(IUnitOfWork unitOfWork)
{
    private readonly IGenericRepository<User> _userRepo = unitOfWork.GetRepository<User>();

    public async Task ExecuteAsync(Guid userId, CancellationToken ct = default)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("User id is required.");

        var user = await _userRepo.GetByIdAsync(userId, ct)
            ?? throw new NotFoundException(nameof(User), userId);

        if (user.Status != UserStatus.Deleted)
            throw new ArgumentException("User need deleted first.");

        await _userRepo.DeleteAsync(userId, ct);
        await unitOfWork.SaveChangesAsync(ct);
    }
}
