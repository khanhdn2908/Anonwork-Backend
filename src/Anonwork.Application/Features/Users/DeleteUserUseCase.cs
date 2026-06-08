using Anonwork.Application.Common.Exceptions;
using Anonwork.Application.Interfaces;
using Anonwork.Domain.Common.Exceptions;
using Anonwork.Domain.Entities;

namespace Anonwork.Application.Features.Users;

public class DeleteUserUseCase(IUnitOfWork unitOfWork)
{
    private readonly IGenericRepository<User> _userRepo = unitOfWork.GetRepository<User>();
    public async Task ExecuteAsync(Guid userId, CancellationToken ct = default)
    {
        var user = await _userRepo.GetByIdWithTrackingAsync(userId, ct)
            ?? throw new NotFoundException("User not found.");

        await _userRepo.DeleteAsync(userId, ct);
       
        await unitOfWork.SaveChangesAsync(ct);
    }
}
