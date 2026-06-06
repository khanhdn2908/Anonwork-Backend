using System.ComponentModel.DataAnnotations;

namespace Anonwork.Application.Features.SubscriptionPlans.DTOs.Requests;

public record UpdateSubscriptionPlanRequestDto(
    [Required, MinLength(3), MaxLength(100)] string Name,
    [MaxLength(100)] string? Slug = null,
    [Range(0, long.MaxValue)] long Price = 0,
    [Range(1, 3650)] int DurationDays = 30, // 1 day to 10 years
    [MaxLength(1000)] string? Features = null,
    bool IsActive = true
);