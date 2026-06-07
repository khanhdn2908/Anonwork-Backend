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

    public async Task<UserListPaginatedResponseDto> ExecuteAsync(
        int page = 1,
        int pageSize = 10,
        CancellationToken ct = default)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 10;
        if (pageSize > 100) pageSize = 100;

        var users = (await _userRepo.GetAllAsync(ct)).OrderByDescending(u => u.CreatedAt);
        var total = await _userRepo.CountAsync(ct);

        var pagedUsers = users.Skip((page - 1) * pageSize).Take(pageSize).ToList();
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

        var userDtos = pagedUsers.Select(u => new UserListResponseDto(
            u.Id,
            u.Username,
            u.AvatarUrl,
            u.Bio,
            u.AnonAlias,
            u.CreatedAt,
            activeSubscriptionByUserId.ContainsKey(u.Id),
            activeSubscriptionByUserId.TryGetValue(u.Id, out var activeSubscription) ? activeSubscription : null
        )).ToList();

        var totalPages = (int)Math.Ceiling(total / (double)pageSize);

        return new UserListPaginatedResponseDto(userDtos, total, page, pageSize, totalPages);
    }
}
