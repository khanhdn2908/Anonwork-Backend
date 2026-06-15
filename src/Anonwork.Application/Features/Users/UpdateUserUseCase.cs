using System.Linq;
using Anonwork.Application.Common.Exceptions;
using Anonwork.Application.Features.Users.DTOs.Requests;
using Anonwork.Application.Features.Users.DTOs.Responses;
using Anonwork.Application.Interfaces;
using Anonwork.Domain.Entities;

namespace Anonwork.Application.Features.Users;

public class UpdateUserUseCase(IUnitOfWork unitOfWork, ICloudinaryService cloudinaryService)
{
    private readonly IGenericRepository<User> _userRepo = unitOfWork.GetRepository<User>();
    private readonly ICloudinaryService _cloudinaryService = cloudinaryService;

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

        string? imageUrls = null;
        if (req.Avatar is not null)
        {
            try
            {
                imageUrls = await _cloudinaryService.UploadImageAsync(req.Avatar, "avatars", ct);
            }
            catch (Exception ex) 
            {
                throw new BadRequestException("Failed to upload images");
            }

            user.AvatarUrl = imageUrls;
        }
            

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
            user.CreatedAt,
            user.UpdatedAt
        );
    }
}
