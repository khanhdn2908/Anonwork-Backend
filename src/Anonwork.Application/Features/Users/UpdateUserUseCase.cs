using Anonwork.Application.Common.Exceptions;
using Anonwork.Application.Features.Users.DTOs;
using Anonwork.Application.Interfaces;
using Anonwork.Domain.Common.Exceptions;
using Anonwork.Domain.Entities;

namespace Anonwork.Application.Features.Users;

public class UpdateUserUseCase(IUnitOfWork unitOfWork)
{
    private readonly IGenericRepository<User> _userRepo = unitOfWork.GetRepository<User>();

    public async Task<GetMeResponseDto> ExecuteAsync(
        Guid userId,
        UpdateUserRequestDto req,
        CancellationToken ct = default)
    {
        var user = await _userRepo.GetByIdAsync(userId, ct)
            ?? throw new NotFoundException("User not found.");

        if (!string.IsNullOrWhiteSpace(req.Username))
        {
            var existingUsers = await _userRepo.FindAsync(u => u.Username == req.Username.ToLower().Trim(), ct);
            var existingUser = existingUsers.FirstOrDefault();
            if (existingUser is not null && existingUser.Id != userId)
                throw new BadRequestException("Username already taken.");

            user.Username = req.Username.ToLower().Trim();
        }

        if (req.Bio is not null)
            user.Bio = req.Bio;

        if (req.AvatarUrl is not null)
            user.AvatarUrl = req.AvatarUrl;

        if (req.IsAnonDefault.HasValue)
            user.IsAnonDefault = req.IsAnonDefault.Value;

        user.UpdatedAt = DateTime.UtcNow;

        await _userRepo.UpdateAsync(user, ct);
        await unitOfWork.SaveChangesAsync(ct);

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
