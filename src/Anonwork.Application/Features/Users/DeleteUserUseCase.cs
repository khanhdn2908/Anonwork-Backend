using Anonwork.Application.Common.Exceptions;
using Anonwork.Application.Interfaces;
using Anonwork.Domain.Common.Exceptions;
using Anonwork.Domain.Entities;

namespace Anonwork.Application.Features.Users;

public class DeleteUserUseCase(IUnitOfWork unitOfWork)
{
    public async Task ExecuteAsync(Guid userId, CancellationToken ct = default)
    {
        var userRepo = unitOfWork.GetRepository<User>();
        var user = await userRepo.GetByIdAsync(userId, ct)
            ?? throw new NotFoundException("User not found.");

        await userRepo.DeleteAsync(userId, ct);
        await unitOfWork.SaveChangesAsync(ct);
    }
}
