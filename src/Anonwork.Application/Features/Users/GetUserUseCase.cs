using Anonwork.Application.Common.Exceptions;
using Anonwork.Application.Features.Users.DTOs.Responses;
using Anonwork.Application.Interfaces;
using Anonwork.Domain.Entities;
using Anonwork.Domain.Enums;
using Microsoft.Extensions.Configuration;

namespace Anonwork.Application.Features.Users;

public class GetUserUseCase(IUnitOfWork unitOfWork, IConfiguration configuration)
{
    private readonly IGenericRepository<User> _userRepo = unitOfWork.GetRepository<User>();
    private readonly IGenericRepository<Follow> _followRepo = unitOfWork.GetRepository<Follow>();
    private readonly IGenericRepository<UserSubscription> _userSubscriptionRepo = unitOfWork.GetRepository<UserSubscription>();
    private readonly string _publicBaseUrl = configuration["R2:PublicBaseUrl"] ?? string.Empty;

    public async Task<UserResponseDto> ExecuteAsync(Guid userId, bool hasPermission, CancellationToken ct = default)
    {
        var user = await _userRepo.GetByIdAsync(userId, ct)
            ?? throw new NotFoundException("User not found.");

        if (!hasPermission && user.Status != UserStatus.Active)
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

        var isAnon = user.IsAnonDefault;
        var displayUsername = isAnon ? user.AnonAlias : user.Username;
        var displayAvatarKey = isAnon ? user.AnonImage?.FileKey : user.AvatarKey;
        var avatarUrl = BuildAvatarUrl(displayAvatarKey);
        var displayBio = isAnon ? null : user.Bio;
        var displayEmail = isAnon ? null : user.Email;

        return new UserResponseDto(
            user.Id,
            displayUsername,
            displayEmail,
            displayAvatarKey,
            avatarUrl,
            displayBio,
            user.AnonAlias,
            user.IsAnonDefault,
            followerCount,
            followingCount,
            userSubscriptionPlanActive,
            user.CreatedAt,
            user.UpdatedAt
        );
    }

    private string BuildAvatarUrl(string? avatarKey)
    {
        var key = string.IsNullOrWhiteSpace(avatarKey) ? "avatars/null.jpg" : avatarKey;
        return string.IsNullOrWhiteSpace(_publicBaseUrl)
            ? key
            : $"{_publicBaseUrl.TrimEnd('/')}/{key.TrimStart('/')}";
    }
}
