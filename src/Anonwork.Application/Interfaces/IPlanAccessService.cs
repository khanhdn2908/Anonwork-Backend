using Anonwork.Domain.Entities;
using Microsoft.AspNetCore.Http;

namespace Anonwork.Application.Interfaces;

public interface IPlanAccessService
{
    Task<SubscriptionPlan?> GetCurrentPlanAsync(Guid userId, CancellationToken ct = default);

    Task<int> GetTodayPostCountAsync(Guid userId, CancellationToken ct = default);

    Task EnsureCanCreatePostAsync(Guid userId, IFormFileCollection? images, IFormFileCollection? files, CancellationToken ct = default);

    Task EnsureCanUseAnonImageAsync(Guid userId, AnonImage anonImage, CancellationToken ct = default);
}
