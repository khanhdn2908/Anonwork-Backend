using Anonwork.Application.Common.Exceptions;
using Anonwork.Application.Features.Users.DTOs;
using Anonwork.Application.Interfaces;
using Anonwork.Domain.Common.Exceptions;

namespace Anonwork.Application.Features.Users;

public class GetMeUseCase(IUserRepository userRepo)
{
    public async Task<GetMeResponseDto> ExecuteAsync(Guid userId, CancellationToken ct = default)
    {
        var user = await userRepo.GetByIdAsync(userId, ct)
            ?? throw new NotFoundException("User not found.");

        return new GetMeResponseDto(
            user.Id,
            user.Username,
            user.Email,
            user.AvatarUrl,
            user.Bio,
            user.AnonAlias,
            user.IsAnonDefault,
            user.Role,
            user.CreatedAt,
            user.UpdatedAt
        );
    }
}
