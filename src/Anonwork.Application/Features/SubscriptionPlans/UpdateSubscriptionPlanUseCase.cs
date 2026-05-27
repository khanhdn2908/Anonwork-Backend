using System.Text.RegularExpressions;
using Anonwork.Application.Common.Exceptions;
using Anonwork.Application.Features.SubscriptionPlans.DTOs;
using Anonwork.Application.Interfaces;
using Anonwork.Domain.Common.Exceptions;

namespace Anonwork.Application.Features.SubscriptionPlans;

public class UpdateSubscriptionPlanUseCase(ISubscriptionPlanRepository subscriptionPlanRepo)
{
    public async Task<SubscriptionPlanResponseDto> ExecuteAsync(
        Guid id,
        UpdateSubscriptionPlanRequestDto request,
        CancellationToken ct = default)
    {
        // Get existing plan
        var existingPlan = await subscriptionPlanRepo.GetByIdAsync(id, ct)
            ?? throw new NotFoundException($"Subscription plan with ID {id} not found.");

        // Generate slug if not provided
        var slug = !string.IsNullOrWhiteSpace(request.Slug) 
            ? request.Slug.Trim().ToLowerInvariant()
            : GenerateSlugFromName(request.Name);

        // Validate slug format
        if (!IsValidSlug(slug))
        {
            throw new BadRequestException("Slug must contain only lowercase letters, numbers, and hyphens.");
        }

        // Check if slug already exists (excluding current plan)
        if (await subscriptionPlanRepo.ExistsBySlugAsync(slug, id, ct))
        {
            throw new ConflictException($"Subscription plan with slug '{slug}' already exists.");
        }

        // Update plan properties
        existingPlan.Name = request.Name.Trim();
        existingPlan.Slug = slug;
        existingPlan.Price = request.Price;
        existingPlan.DurationDays = request.DurationDays;
        existingPlan.Features = request.Features?.Trim();
        existingPlan.IsActive = request.IsActive;

        await subscriptionPlanRepo.UpdateAsync(existingPlan, ct);

        return new SubscriptionPlanResponseDto(
            existingPlan.Id,
            existingPlan.Name,
            existingPlan.Slug,
            existingPlan.Price,
            existingPlan.DurationDays,
            existingPlan.Features,
            existingPlan.IsActive,
            existingPlan.CreatedAt
        );
    }

    private static string GenerateSlugFromName(string name)
    {
        // Convert to lowercase and replace spaces with hyphens
        var slug = name.Trim().ToLowerInvariant();
        
        // Remove special characters and replace with hyphens
        slug = Regex.Replace(slug, @"[^a-z0-9\s-]", "");
        
        // Replace multiple spaces/hyphens with single hyphen
        slug = Regex.Replace(slug, @"[\s-]+", "-");
        
        // Remove leading/trailing hyphens
        slug = slug.Trim('-');
        
        return slug;
    }

    private static bool IsValidSlug(string slug)
    {
        // Slug should only contain lowercase letters, numbers, and hyphens
        // Should not start or end with hyphen
        return Regex.IsMatch(slug, @"^[a-z0-9]+(?:-[a-z0-9]+)*$");
    }
}