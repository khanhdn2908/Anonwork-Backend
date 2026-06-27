using Anonwork.Application.Common.Exceptions;
using Anonwork.Application.Features.Users.DTOs.Responses;
using Anonwork.Application.Interfaces;
using Anonwork.Domain.Entities;
using Anonwork.Domain.Enums;

namespace Anonwork.Application.Features.Users;

public class GetUserUseCase(IUnitOfWork unitOfWork, IR2Service r2Service)
{
    private readonly IGenericRepository<User> _userRepo = unitOfWork.GetRepository<User>();
    private readonly IGenericRepository<Follow> _followRepo = unitOfWork.GetRepository<Follow>();
    private readonly IGenericRepository<UserSubscription> _userSubscriptionRepo = unitOfWork.GetRepository<UserSubscription>();
    private readonly IR2Service _r2Service = r2Service;

    public async Task<UserResponseDto> ExecuteAsync(Guid userId, bool hasPermission, CancellationToken ct = default)
    {
        var user = await _userRepo.FindWithIncludesAsync(
            u => u.Id == userId,
            u => u.AnonImage
        );
        
        var foundUser = user.FirstOrDefault()
            ?? throw new NotFoundException("User not found.");

        if (!hasPermission && foundUser.Status != UserStatus.Active)
            throw new NotFoundException("User not found.");

        var followerCount = await _followRepo.CountAsync(
            f => f.FollowingId == userId && f.Following.Status == UserStatus.Active,
            ct);

        var followingCount = await _followRepo.CountAsync(
            f => f.FollowerId == userId && f.Follower.Status == UserStatus.Active,
            ct);

        var userSubscriptionPlanActive = (await _userSubscriptionRepo.FindAsync(
                us => us.UserId == userId && us.Status == SubscriptionStatus.Active && us.ExpiresAt > DateTime.UtcNow,
                ct))
            .Select(us => us.Plan?.Name)
            .Where(name => !string.IsNullOrWhiteSpace(name))
            .Select(name => name!)
            .Distinct()
            .ToList();

        var isAnon = foundUser.IsAnonDefault;
        var displayUsername = isAnon ? foundUser.AnonAlias : foundUser.Username;
        var avatarKey = isAnon ? foundUser.AnonImage?.FileKey : foundUser.AvatarKey;
        var avatarUrl = string.IsNullOrWhiteSpace(avatarKey)
            ? _r2Service.GetPublicUrl("avatars/null.jpg")
            : _r2Service.GetPublicUrl(avatarKey);
        var displayBio = isAnon ? null : foundUser.Bio;
        var displayEmail = isAnon ? null : foundUser.Email;

        return new UserResponseDto(
            foundUser.Id,
            displayUsername,
            displayEmail,
            avatarKey,
            avatarUrl,
            displayBio,
            foundUser.AnonAlias,
            foundUser.IsAnonDefault,
            followerCount,
            followingCount,
            userSubscriptionPlanActive,
            foundUser.CreatedAt,
            foundUser.UpdatedAt
        );
    }
}
