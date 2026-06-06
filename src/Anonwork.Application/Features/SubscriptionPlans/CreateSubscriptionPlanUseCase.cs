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
        // Generate slug if not provided
        var slug = !string.IsNullOrWhiteSpace(request.Slug)
            ? request.Slug.Trim().ToLowerInvariant()
            : GenerateSlugFromName(request.Name);

        // Validate slug format
        if (!IsValidSlug(slug))
        {
            throw new BadRequestException("Slug must contain only lowercase letters, numbers, and hyphens.");
        }

        // Check if slug already exists
        if (await _subscriptionPlanRepo.ExistsAsync(p => p.Slug == slug, ct))
        {
            throw new ConflictException($"Subscription plan with slug '{slug}' already exists.");
        }

        // Create new subscription plan
        var plan = new SubscriptionPlan
        {
            Id = Guid.NewGuid(),
            Name = request.Name.Trim(),
            Slug = slug,
            Price = request.Price,
            DurationDays = request.DurationDays,
            Features = request.Features?.Trim(),
            IsActive = request.IsActive,
            CreatedAt = DateTime.UtcNow
        };

        var createdPlan = await _subscriptionPlanRepo.AddAsync(plan, ct);
        await unitOfWork.SaveChangesAsync(ct);

        return new SubscriptionPlanResponseDto(
            createdPlan.Id,
            createdPlan.Name,
            createdPlan.Slug,
            createdPlan.Price,
            createdPlan.DurationDays,
            createdPlan.Features,
            createdPlan.IsActive,
            createdPlan.CreatedAt
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