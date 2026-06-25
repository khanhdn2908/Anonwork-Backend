using System.Linq;
using Anonwork.Application.Common.Exceptions;
using Anonwork.Application.Features.Users.DTOs.Requests;
using Anonwork.Application.Features.Users.DTOs.Responses;
using Anonwork.Application.Interfaces;
using Anonwork.Domain.Entities;

namespace Anonwork.Application.Features.Users;

public class UpdateUserUseCase(IUnitOfWork unitOfWork, IR2Service r2Service)
{
    private readonly IGenericRepository<User> _userRepo = unitOfWork.GetRepository<User>();
    private readonly IR2Service _r2Service = r2Service;

    public async Task<UpdateUserResponseDto> ExecuteAsync(
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

        if (req.Avatar is not null)
        {
            var oldAvatarKey = user.AvatarKey;

            try
            {
                var uploadedFile = await _r2Service.UploadFileAsync(req.Avatar, "avatars", ct);
                user.AvatarKey = uploadedFile.FileKey;

                if (!string.IsNullOrWhiteSpace(oldAvatarKey) &&
                    oldAvatarKey != _r2Service.GetDefaultAvatarKey() &&
                    oldAvatarKey != user.AvatarKey)
                {
                    await _r2Service.DeleteFileAsync(oldAvatarKey, ct);
                }
            }
            catch
            {
                throw new BadRequestException("Failed to upload image.");
            }
        }

        user.UpdatedAt = DateTime.UtcNow;

        await _userRepo.UpdateAsync(user, ct);
        await unitOfWork.SaveChangesAsync(ct);

        var defaultAvatarKey = _r2Service.GetDefaultAvatarKey();

        return new UpdateUserResponseDto(
            user.Username,
            user.Bio,
            user.AvatarKey,
            _r2Service.GetPublicUrl(user.AvatarKey ?? defaultAvatarKey)
        );
    }
}
