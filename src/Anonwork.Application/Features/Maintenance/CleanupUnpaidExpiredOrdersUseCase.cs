using Anonwork.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Anonwork.Application.Features.Maintenance;

public class CleanupUnpaidExpiredOrdersUseCase(IAppDbContext dbContext)
{
    private static readonly TimeSpan GracePeriod = TimeSpan.FromDays(7);

    public async Task<int> ExecuteAsync(CancellationToken ct = default)
    {
        var cutoff = DateTime.UtcNow.Add(GracePeriod.Negate());

        var ordersToDelete = await dbContext.Orders
            .Where(order =>
                order.PaidAt == null &&
                order.ExpiresAt != null &&
                order.ExpiresAt < cutoff)
            .ToListAsync(ct);

        if (ordersToDelete.Count == 0)
            return 0;

        dbContext.Orders.RemoveRange(ordersToDelete);
        await dbContext.SaveChangesAsync(ct);

        return ordersToDelete.Count;
    }
}
