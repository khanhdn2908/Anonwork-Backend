using Anonwork.Application.Interfaces;
using Anonwork.Domain.Enums;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Anonwork.Infrastructure.Services;

/// <summary>
/// Background service để auto-renew subscriptions khi hết hạn
/// </summary>
public class SubscriptionRenewalService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<SubscriptionRenewalService> _logger;
    private readonly TimeSpan _checkInterval = TimeSpan.FromHours(1);

    public SubscriptionRenewalService(
        IServiceProvider serviceProvider,
        ILogger<SubscriptionRenewalService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("SubscriptionRenewalService started");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await RenewExpiredSubscriptionsAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in subscription renewal: {ex.Message}");
            }

            await Task.Delay(_checkInterval, stoppingToken);
        }

        _logger.LogInformation("SubscriptionRenewalService stopped");
    }

    private async Task RenewExpiredSubscriptionsAsync(CancellationToken ct)
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var subscriptionRepo = scope.ServiceProvider.GetRequiredService<IUserSubscriptionRepository>();
            var planRepo = scope.ServiceProvider.GetRequiredService<ISubscriptionPlanRepository>();

            // ── Get expired subscriptions ───────────────
            var expiredSubscriptions = await subscriptionRepo.GetExpiredSubscriptionsAsync(ct);

            if (expiredSubscriptions.Count == 0)
            {
                _logger.LogInformation("No expired subscriptions to renew");
                return;
            }

            _logger.LogInformation($"Found {expiredSubscriptions.Count} expired subscriptions to renew");

            // ── Renew each subscription ─────────────────
            foreach (var subscription in expiredSubscriptions)
            {
                try
                {
                    var plan = await planRepo.GetByIdAsync(subscription.PlanId, ct);
                    if (plan is null)
                    {
                        _logger.LogWarning($"Plan not found for subscription {subscription.Id}");
                        continue;
                    }

                    // ── Update subscription ─────────────
                    subscription.Status = SubscriptionStatus.Active;
                    subscription.StartedAt = DateTime.UtcNow;
                    subscription.ExpiresAt = DateTime.UtcNow.AddDays(plan.DurationDays);

                    await subscriptionRepo.UpdateAsync(subscription, ct);

                    _logger.LogInformation($"Renewed subscription {subscription.Id} for user {subscription.UserId}");
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error renewing subscription {subscription.Id}: {ex.Message}");
                }
            }
        }
    }
}
