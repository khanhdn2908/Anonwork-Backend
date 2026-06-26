using System.ComponentModel.DataAnnotations;

namespace Anonwork.Application.Features.SubscriptionPlans.DTOs.Requests;

public record UpdateSubscriptionPlanRequestDto(
    [Required, MinLength(3), MaxLength(100)] string Name,
    [MaxLength(100)] string? Slug = null,
    [MaxLength(1000)] string? Description = null,
    [Range(0, long.MaxValue)] long Price = 0,
    [Range(1, 3650)] int DurationDays = 30,
    [Range(0, int.MaxValue)] int MaxPostsPerDay = 0,
    [Range(0, int.MaxValue)] int MaxUploadsPerDay = 0,
    [Range(0, int.MaxValue)] int MaxPostFileSizeMb = 0,
    [Range(0, int.MaxValue)] int MaxPostImageCount = 0,
    [Range(0, int.MaxValue)] int MaxPostMediaCount = 0,
    bool CanAttachMediaToPost = false,
    bool CanUploadPostFiles = false,
    bool CanUseExclusiveAnonImages = false,
    bool CanUsePremiumFeatures = false,
    bool IsActive = true
);