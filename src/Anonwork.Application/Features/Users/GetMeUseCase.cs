using Anonwork.Application.Common.Exceptions;
using Anonwork.Application.Features.Users.DTOs.Responses;
using Anonwork.Application.Interfaces;
using Anonwork.Domain.Entities;
using Anonwork.Domain.Enums;

namespace Anonwork.Application.Features.Users;

public class GetMeUseCase(IUnitOfWork unitOfWork)
{
    private readonly IGenericRepository<User> _userRepo = unitOfWork.GetRepository<User>();
    private readonly IGenericRepository<Follow> _followRepo = unitOfWork.GetRepository<Follow>();
    private readonly IGenericRepository<UserSubscription> _userSubscriptionRepo = unitOfWork.GetRepository<UserSubscription>();

    public async Task<GetMeResponseDto> ExecuteAsync(Guid userId, CancellationToken ct = default)
    {
        var user = await _userRepo.GetByIdAsync(userId, ct)
            ?? throw new NotFoundException("User not found.");

        var followerCount = await _followRepo.CountAsync(
            f => f.FollowingId == userId && f.Following.Status == UserStatus.Active,
            ct);

        var followingCount = await _followRepo.CountAsync(
            f => f.FollowerId == userId && f.Follower.Status == UserStatus.Active,
            ct);

        var anonImageUrl = user.AnonImage?.FileKey ?? string.Empty;

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
}
