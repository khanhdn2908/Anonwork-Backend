using Anonwork.Application.Common.Exceptions;
using Anonwork.Application.Features.Users.DTOs.Responses;
using Anonwork.Application.Interfaces;
using Anonwork.Domain.Common.Exceptions;

namespace Anonwork.Application.Features.Users;

public class GetMeUseCase(IUnitOfWork unitOfWork)
{
    private readonly IGenericRepository<Anonwork.Domain.Entities.User> _userRepo = unitOfWork.GetRepository<Anonwork.Domain.Entities.User>();

    public async Task<GetMeResponseDto> ExecuteAsync(Guid userId, CancellationToken ct = default)
    {
        var user = await _userRepo.GetByIdAsync(userId, ct)
            ?? throw new NotFoundException("User not found.");

        return new GetMeResponseDto(
            user.Id,
            user.Username,
            user.Email,
            user.AvatarUrl,
            user.Bio,
            user.AnonAlias,
            user.IsAnonDefault,
            user.CreatedAt,
            user.UpdatedAt
        );
    }
}
