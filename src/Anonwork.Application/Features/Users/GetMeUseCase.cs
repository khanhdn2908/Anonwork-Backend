using Anonwork.Application.Common.Exceptions;
using Anonwork.Application.Features.Users.DTOs.Responses;
using Anonwork.Application.Interfaces;
using Anonwork.Domain.Entities;
using Anonwork.Domain.Enums;

namespace Anonwork.Application.Features.Users;

public class GetMeUseCase(IUnitOfWork unitOfWork, IR2Service r2Service)
{
    private readonly IGenericRepository<User> _userRepo = unitOfWork.GetRepository<User>();
    private readonly IGenericRepository<Follow> _followRepo = unitOfWork.GetRepository<Follow>();
    private readonly IGenericRepository<UserSubscription> _userSubscriptionRepo = unitOfWork.GetRepository<UserSubscription>();
    private readonly IR2Service _r2Service = r2Service;

    public async Task<GetMeResponseDto> ExecuteAsync(Guid userId, CancellationToken ct = default)
    {
        var users = await _userRepo.FindWithIncludesAsync(
            u => u.Id == userId,
            u => u.AnonImage
        );

        var user = users.FirstOrDefault()
            ?? throw new NotFoundException("User not found.");

        var followerCount = await _followRepo.CountAsync(
            f => f.FollowingId == userId && f.Following.Status == UserStatus.Active,
            ct);

        var followingCount = await _followRepo.CountAsync(
            f => f.FollowerId == userId && f.Follower.Status == UserStatus.Active,
            ct);

        var anonImageUrl = !string.IsNullOrWhiteSpace(user.AnonImage?.FileKey)
            ? _r2Service.GetPublicUrl(user.AnonImage.FileKey)
            : string.Empty;

        var userSubscriptionPlanActive = (await _userSubscriptionRepo.FindAsync(
                us => us.UserId == userId && us.Status == SubscriptionStatus.Active && us.ExpiresAt > DateTime.UtcNow,
                ct))
            .Select(us => us.Plan?.Name)
            .Where(name => !string.IsNullOrWhiteSpace(name))
            .Select(name => name!)
            .Distinct()
            .ToList();

        return new GetMeResponseDto(
            user.Id,
            user.Username,
            user.Email,
            user.AvatarKey,
            BuildAvatarUrl(user.AvatarKey),
            user.Bio,
            user.AnonAlias,
            user.IsAnonDefault,
            followerCount,
            followingCount,
            anonImageUrl,
            userSubscriptionPlanActive,
            user.CreatedAt,
            user.UpdatedAt
        );
    }

    private string BuildAvatarUrl(string? avatarKey)
    {
        return _r2Service.GetPublicUrl(
            string.IsNullOrWhiteSpace(avatarKey) ? "avatars/null.jpg" : avatarKey
        );
    }
}

