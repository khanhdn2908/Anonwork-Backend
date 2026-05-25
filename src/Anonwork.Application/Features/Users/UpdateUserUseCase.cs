using Anonwork.Application.Common.Exceptions;
using Anonwork.Application.Features.Users.DTOs;
using Anonwork.Application.Interfaces;
using Anonwork.Domain.Common.Exceptions;

namespace Anonwork.Application.Features.Users;

public class UpdateUserUseCase(IUserRepository userRepo)
{
    public async Task<GetMeResponseDto> ExecuteAsync(
        Guid userId,
        UpdateUserRequestDto req,
        CancellationToken ct = default)
    {
        var user = await userRepo.GetByIdAsync(userId, ct)
            ?? throw new NotFoundException("User not found.");

        // Update fields if provided
        if (!string.IsNullOrWhiteSpace(req.Username))
        {
            // Check if username already exists (and it's not the same user)
            var existingUser = await userRepo.GetByUsernameAsync(req.Username, ct);
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

        await userRepo.UpdateAsync(user, ct);

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
