using System.ComponentModel.DataAnnotations;
using Anonwork.Domain.Enums;

namespace Anonwork.Application.Features.UserSubscriptions.DTOs.Requests;

public record CreateUserSubscriptionRequestDto(
    [Required] Guid UserId,
    [Required] Guid PlanId,
    [Required] Guid OrderId,
    SubscriptionStatus Status = SubscriptionStatus.Active,
    DateTime? StartedAt = null
);

public record UpdateUserSubscriptionRequestDto(
    SubscriptionStatus? Status = null,
    DateTime? ExpiresAt = null
);

public record GetUserSubscriptionsByUserIdRequestDto(
    [Required] Guid UserId,
    int Page = 1,
    int PageSize = 10
);