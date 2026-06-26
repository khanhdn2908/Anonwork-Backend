using System.Text.RegularExpressions;
using Anonwork.Application.Common.Exceptions;
using Anonwork.Application.Features.SubscriptionPlans.DTOs.Requests;
using Anonwork.Application.Features.SubscriptionPlans.DTOs.Responses;
using Anonwork.Application.Interfaces;
using Anonwork.Domain.Entities;

namespace Anonwork.Application.Features.SubscriptionPlans;

public class CreateSubscriptionPlanUseCase(IUnitOfWork unitOfWork)
{
    private readonly IGenericRepository<SubscriptionPlan> _subscriptionPlanRepo = unitOfWork.GetRepository<SubscriptionPlan>();

    public async Task<SubscriptionPlanResponseDto> ExecuteAsync(
        CreateSubscriptionPlanRequestDto request,
        CancellationToken ct = default)
    {
        var slug = !string.IsNullOrWhiteSpace(request.Slug)
            ? request.Slug.Trim().ToLowerInvariant()
            : GenerateSlugFromName(request.Name);

        if (!IsValidSlug(slug))
            throw new BadRequestException("Slug must contain only lowercase letters, numbers, and hyphens.");

        if (await _subscriptionPlanRepo.ExistsAsync(p => p.Slug == slug, ct))
            throw new ConflictException($"Subscription plan with slug '{slug}' already exists.");

        var plan = new SubscriptionPlan
        {
            Id = Guid.NewGuid(),
            Name = request.Name.Trim(),
            Slug = slug,
            Description = request.Description?.Trim(),
            Price = request.Price,
            DurationDays = request.DurationDays,
            MaxPostsPerDay = request.MaxPostsPerDay,
            MaxUploadsPerDay = request.MaxUploadsPerDay,
            MaxPostFileSizeMb = request.MaxPostFileSizeMb,
            MaxPostImageCount = request.MaxPostImageCount,
            MaxPostMediaCount = request.MaxPostMediaCount,
            CanAttachMediaToPost = request.CanAttachMediaToPost,
            CanUploadPostFiles = request.CanUploadPostFiles,
            CanUseExclusiveAnonImages = request.CanUseExclusiveAnonImages,
            CanUsePremiumFeatures = request.CanUsePremiumFeatures,
            IsActive = request.IsActive,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var createdPlan = await _subscriptionPlanRepo.AddAsync(plan, ct);
        await unitOfWork.SaveChangesAsync(ct);

        return MapToResponse(createdPlan);
    }

    private static SubscriptionPlanResponseDto MapToResponse(SubscriptionPlan plan)
        => new(
            plan.Id,
            plan.Name,
            plan.Slug,
            plan.Description,
            plan.Price,
            plan.DurationDays,
            plan.MaxPostsPerDay,
            plan.MaxUploadsPerDay,
            plan.MaxPostFileSizeMb,
            plan.MaxPostImageCount,
            plan.MaxPostMediaCount,
            plan.CanAttachMediaToPost,
            plan.CanUploadPostFiles,
            plan.CanUseExclusiveAnonImages,
            plan.CanUsePremiumFeatures,
            plan.IsActive,
            plan.CreatedAt,
            plan.UpdatedAt);

    private static string GenerateSlugFromName(string name)
    {
        var slug = name.Trim().ToLowerInvariant();
        slug = Regex.Replace(slug, @"[^a-z0-9\s-]", "");
        slug = Regex.Replace(slug, @"[\s-]+", "-");
        return slug.Trim('-');
    }

    private static bool IsValidSlug(string slug)
        => Regex.IsMatch(slug, @"^[a-z0-9]+(?:-[a-z0-9]+)*$");
}
