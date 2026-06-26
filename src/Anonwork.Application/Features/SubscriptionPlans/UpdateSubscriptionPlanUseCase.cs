using System.Text.RegularExpressions;
using Anonwork.Application.Common.Exceptions;
using Anonwork.Application.Features.SubscriptionPlans.DTOs.Requests;
using Anonwork.Application.Features.SubscriptionPlans.DTOs.Responses;
using Anonwork.Application.Interfaces;
using Anonwork.Domain.Entities;

namespace Anonwork.Application.Features.SubscriptionPlans;

public class UpdateSubscriptionPlanUseCase(IUnitOfWork unitOfWork)
{
    private readonly IGenericRepository<SubscriptionPlan> _subscriptionPlanRepo = unitOfWork.GetRepository<SubscriptionPlan>();

    public async Task<SubscriptionPlanResponseDto> ExecuteAsync(
        Guid id,
        UpdateSubscriptionPlanRequestDto request,
        CancellationToken ct = default)
    {
        var existingPlan = await _subscriptionPlanRepo.GetByIdAsync(id, ct)
            ?? throw new NotFoundException($"Subscription plan with ID {id} not found.");

        var slug = !string.IsNullOrWhiteSpace(request.Slug)
            ? request.Slug.Trim().ToLowerInvariant()
            : GenerateSlugFromName(request.Name);

        if (!IsValidSlug(slug))
            throw new BadRequestException("Slug must contain only lowercase letters, numbers, and hyphens.");

        if (await _subscriptionPlanRepo.ExistsAsync(s => s.Slug == slug && s.Id != id, ct))
            throw new ConflictException($"Subscription plan with slug '{slug}' already exists.");

        existingPlan.Name = request.Name.Trim();
        existingPlan.Slug = slug;
        existingPlan.Description = request.Description?.Trim();
        existingPlan.Price = request.Price;
        existingPlan.DurationDays = request.DurationDays;
        existingPlan.MaxPostsPerDay = request.MaxPostsPerDay;
        existingPlan.MaxUploadsPerDay = request.MaxUploadsPerDay;
        existingPlan.MaxPostFileSizeMb = request.MaxPostFileSizeMb;
        existingPlan.MaxPostImageCount = request.MaxPostImageCount;
        existingPlan.MaxPostMediaCount = request.MaxPostMediaCount;
        existingPlan.CanAttachMediaToPost = request.CanAttachMediaToPost;
        existingPlan.CanUploadPostFiles = request.CanUploadPostFiles;
        existingPlan.CanUseExclusiveAnonImages = request.CanUseExclusiveAnonImages;
        existingPlan.CanUsePremiumFeatures = request.CanUsePremiumFeatures;
        existingPlan.IsActive = request.IsActive;
        existingPlan.UpdatedAt = DateTime.UtcNow;

        await _subscriptionPlanRepo.UpdateAsync(existingPlan, ct);
        await unitOfWork.SaveChangesAsync(ct);

        return MapToResponse(existingPlan);
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
