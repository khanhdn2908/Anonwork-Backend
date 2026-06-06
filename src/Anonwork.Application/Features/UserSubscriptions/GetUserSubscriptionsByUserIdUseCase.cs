using Anonwork.Application.Features.UserSubscriptions.DTOs.Requests;
using Anonwork.Application.Features.UserSubscriptions.DTOs.Responses;
using Anonwork.Application.Interfaces;
using Anonwork.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Anonwork.Application.Features.UserSubscriptions;

public class GetUserSubscriptionsByUserIdUseCase(IUnitOfWork unitOfWork)
{
    private readonly IGenericRepository<UserSubscription> _userSubscriptionRepository = unitOfWork.GetRepository<UserSubscription>();
    private readonly IGenericRepository<User> _userRepository = unitOfWork.GetRepository<User>();
    private readonly IGenericRepository<SubscriptionPlan> _subscriptionPlanRepository = unitOfWork.GetRepository<SubscriptionPlan>();

    public async Task<UserSubscriptionListPaginatedResponseDto> ExecuteAsync(
        GetUserSubscriptionsByUserIdRequestDto request,
        CancellationToken ct = default)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId, ct);
        if (user == null)
            throw new ArgumentException($"User with ID {request.UserId} not found");

        var page = request.Page < 1 ? 1 : request.Page;
        var pageSize = request.PageSize < 1 ? 10 : request.PageSize;

        var allSubscriptions = await _userSubscriptionRepository.GetQueryableNoTracking()
            .Where(s => s.UserId == request.UserId)
            .OrderByDescending(s => s.CreatedAt)
            .ToListAsync(ct);

        var totalCount = allSubscriptions.Count;
        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        var paginatedSubscriptions = allSubscriptions
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        var subscriptionDtos = new List<UserSubscriptionListResponseDto>();

        foreach (var subscription in paginatedSubscriptions)
        {
            var plan = await _subscriptionPlanRepository.GetByIdAsync(subscription.PlanId, ct);

            subscriptionDtos.Add(new UserSubscriptionListResponseDto(
                subscription.Id,
                subscription.UserId,
                subscription.PlanId,
                subscription.OrderId,
                subscription.Status,
                subscription.StartedAt,
                subscription.ExpiresAt,
                subscription.CreatedAt,
                user.Username,
                plan?.Name
            ));
        }

        return new UserSubscriptionListPaginatedResponseDto(
            subscriptionDtos,
            totalCount,
            page,
            pageSize,
            totalPages
        );
    }
}