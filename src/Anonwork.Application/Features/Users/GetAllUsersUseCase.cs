using Anonwork.Application.Features.Users.DTOs.Responses;
using Anonwork.Application.Interfaces;
using Anonwork.Domain.Entities;
using Anonwork.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Anonwork.Application.Features.Users;

public class GetAllUsersUseCase(IUnitOfWork unitOfWork)
{
    private readonly IGenericRepository<User> _userRepo = unitOfWork.GetRepository<User>();
    private readonly IGenericRepository<UserSubscription> _userSubscriptionRepo = unitOfWork.GetRepository<UserSubscription>();
    private readonly IGenericRepository<SubscriptionPlan> _subscriptionPlanRepo = unitOfWork.GetRepository<SubscriptionPlan>();
    private readonly IGenericRepository<Follow> _followRepo = unitOfWork.GetRepository<Follow>();

    public async Task<UserListPaginatedResponseDto> ExecuteAsync(
        bool hasPermission,
        string? search = null,
        int page = 1,
        int pageSize = 10,
        CancellationToken ct = default)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 10;
        if (pageSize > 100) pageSize = 100;

        var users = _userRepo.GetQueryableNoTracking();

        if (!hasPermission)
            users = users.Where(u => u.Status == UserStatus.Active);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var keyword = search.Trim().ToLower();
            users = users.Where(u =>
                u.Username.ToLower().Contains(keyword) ||
                u.Email.ToLower().Contains(keyword) ||
                u.AnonAlias.ToLower().Contains(keyword));
        }

        users = users.Include(u => u.AnonImage).OrderByDescending(u => u.CreatedAt);

        var total = await users.CountAsync(ct);

        var pagedUsers = await users
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        var userIds = pagedUsers.Select(u => u.Id).ToList();

        var activeSubscriptions = await _userSubscriptionRepo.GetQueryableNoTracking()
            .Where(s => userIds.Contains(s.UserId)
                && s.Status == SubscriptionStatus.Active
                && s.ExpiresAt > DateTime.UtcNow)
            .OrderByDescending(s => s.CreatedAt)
            .ToListAsync(ct);

        var planIds = activeSubscriptions.Select(s => s.PlanId).Distinct().ToList();
        var plans = await _subscriptionPlanRepo.GetQueryableNoTracking()
            .Where(p => planIds.Contains(p.Id))
            .ToDictionaryAsync(p => p.Id, ct);

        var activeSubscriptionByUserId = new Dictionary<Guid, UserActiveSubscriptionDto>();

        foreach (var subscription in activeSubscriptions)
        {
            if (activeSubscriptionByUserId.ContainsKey(subscription.UserId))
            {
                continue;
            }

            if (!plans.TryGetValue(subscription.PlanId, out var plan))
            {
                continue;
            }

            activeSubscriptionByUserId[subscription.UserId] = new UserActiveSubscriptionDto(
                subscription.Id,
                subscription.PlanId,
                plan.Name,
                plan.Slug,
                plan.Price,
                plan.DurationDays,
                subscription.StartedAt,
                subscription.ExpiresAt
            );
        }

        var followerCounts = await _followRepo.GetQueryableNoTracking()
            .Where(f => userIds.Contains(f.FollowingId))
            .GroupBy(f => f.FollowingId)
            .Select(g => new { UserId = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.UserId, x => x.Count, ct);

        var followingCounts = await _followRepo.GetQueryableNoTracking()
            .Where(f => userIds.Contains(f.FollowerId))
            .GroupBy(f => f.FollowerId)
            .Select(g => new { UserId = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.UserId, x => x.Count, ct);

        var userDtos = pagedUsers.Select(u => new UserListResponseDto(
            u.Id,
            u.IsAnonDefault ? u.AnonAlias : u.Username,
            u.IsAnonDefault ? null : u.Email,
            u.IsAnonDefault ? u.AnonImage?.ImageUrl : u.AvatarUrl,
            u.IsAnonDefault ? null : u.Bio,
            u.AnonAlias,
            u.IsAnonDefault,
            followerCounts.TryGetValue(u.Id, out var followerCount) ? followerCount : 0,
            followingCounts.TryGetValue(u.Id, out var followingCount) ? followingCount : 0,
            activeSubscriptionByUserId.ContainsKey(u.Id),
            activeSubscriptionByUserId.TryGetValue(u.Id, out var activeSubscription) ? activeSubscription : null,
            u.CreatedAt,
            u.UpdatedAt
        )).ToList();

        var totalPages = (int)Math.Ceiling(total / (double)pageSize);

        return new UserListPaginatedResponseDto(userDtos, total, page, pageSize, totalPages);
    }
}
