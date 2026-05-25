using Anonwork.Application.Common.Exceptions;
using Anonwork.Application.Interfaces;
using Anonwork.Domain.Common.Exceptions;

namespace Anonwork.Application.Features.Users;

public class DeleteUserUseCase(IUserRepository userRepo)
{
    public async Task ExecuteAsync(Guid userId, CancellationToken ct = default)
    {
        var user = await userRepo.GetByIdAsync(userId, ct)
            ?? throw new NotFoundException("User not found.");

        // Soft delete - mark as deleted by setting a flag or removing from active users
        // For now, we'll just delete the user record
        // In a real app, you might want to soft-delete instead
        await userRepo.DeleteAsync(userId, ct);
    }
}
