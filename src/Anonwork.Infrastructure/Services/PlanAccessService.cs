using Anonwork.Application.Common.Exceptions;
using Anonwork.Application.Interfaces;
using Anonwork.Domain.Entities;
using Anonwork.Domain.Enums;
using Microsoft.AspNetCore.Http;

namespace Anonwork.Infrastructure.Services;

public class PlanAccessService(IUnitOfWork unitOfWork) : IPlanAccessService
{
    private readonly IGenericRepository<UserSubscription> _subscriptionRepo = unitOfWork.GetRepository<UserSubscription>();
    private readonly IGenericRepository<Post> _postRepo = unitOfWork.GetRepository<Post>();

    public async Task<SubscriptionPlan?> GetCurrentPlanAsync(Guid userId, CancellationToken ct = default)
    {
        var now = DateTime.UtcNow;
        var subscription = await _subscriptionRepo.FindSingleAsync(
            s => s.UserId == userId && s.Status == SubscriptionStatus.Active && s.ExpiresAt > now,
            ct);

        return subscription?.Plan;
    }

    public async Task<int> GetTodayPostCountAsync(Guid userId, CancellationToken ct = default)
    {
        var today = DateTime.UtcNow.Date;
        var tomorrow = today.AddDays(1);
        return await _postRepo.CountAsync(p => p.AuthorId == userId && p.CreatedAt >= today && p.CreatedAt < tomorrow, ct);
    }

    public async Task EnsureCanCreatePostAsync(Guid userId, IFormFileCollection? images, IFormFileCollection? files, CancellationToken ct = default)
    {
        var plan = await GetCurrentPlanAsync(userId, ct);
        if (plan is null)
        {
            if (images is not null && images.Count > 0)
                throw new BadRequestException("Free plan does not allow post images.");

            if (files is not null && files.Count > 0)
                throw new BadRequestException("Free plan does not allow post files.");

            if (await GetTodayPostCountAsync(userId, ct) >= 1)
                throw new BadRequestException("Free plan allows only 1 post per day.");

            return;
        }

        if (!plan.CanUsePremiumFeatures)
        {
            if (images is not null && images.Count > 0)
                throw new BadRequestException("Current plan does not allow post images.");

            if (files is not null && files.Count > 0)
                throw new BadRequestException("Current plan does not allow post files.");
        }

        if (plan.MaxPostsPerDay > 0 && await GetTodayPostCountAsync(userId, ct) >= plan.MaxPostsPerDay)
            throw new BadRequestException("You have reached today's post limit for your current plan.");

        if (files is not null && files.Count > 0)
        {
            if (!plan.CanUploadPostFiles)
                throw new BadRequestException("Current plan does not allow uploading files in posts.");

            if (plan.MaxPostFileSizeMb > 0)
            {
                var maxBytes = plan.MaxPostFileSizeMb * 1024L * 1024L;
                foreach (var file in files)
                {
                    if (file.Length > maxBytes)
                        throw new BadRequestException($"File '{file.FileName}' exceeds the maximum size of {plan.MaxPostFileSizeMb} MB.");
                }
            }
        }

        if (images is not null && images.Count > 0)
        {
            if (plan.MaxPostImageCount > 0 && images.Count > plan.MaxPostImageCount)
                throw new BadRequestException($"Your plan allows only {plan.MaxPostImageCount} images per post.");
        }

        var totalMedia = (images?.Count ?? 0) + (files?.Count ?? 0);
        if (plan.MaxPostMediaCount > 0 && totalMedia > plan.MaxPostMediaCount)
            throw new BadRequestException($"Your plan allows only {plan.MaxPostMediaCount} media items per post.");
    }

    public Task EnsureCanUseAnonImageAsync(Guid userId, AnonImage anonImage, CancellationToken ct = default)
    {
        if (!anonImage.IsActive)
            throw new BadRequestException("Anon image is inactive.");

        if (!anonImage.IsExclusive)
            return Task.CompletedTask;

        return EnsureExclusiveAnonImageAccessAsync(userId, ct);
    }

    private async Task EnsureExclusiveAnonImageAccessAsync(Guid userId, CancellationToken ct)
    {
        var plan = await GetCurrentPlanAsync(userId, ct);
        if (plan is null || !plan.CanUseExclusiveAnonImages)
            throw new BadRequestException("Your plan does not allow using exclusive anon images.");
    }
}
