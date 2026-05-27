using Anonwork.Application.Common.Exceptions;
using Anonwork.Application.Interfaces;
using Anonwork.Domain.Common.Exceptions;

namespace Anonwork.Application.Features.SubscriptionPlans;

public class DeleteSubscriptionPlanUseCase(ISubscriptionPlanRepository subscriptionPlanRepo)
{
    public async Task ExecuteAsync(Guid id, CancellationToken ct = default)
    {
        // Check if plan exists
        var existingPlan = await subscriptionPlanRepo.GetByIdAsync(id, ct)
            ?? throw new NotFoundException($"Subscription plan with ID {id} not found.");

        // Note: In a real application, you might want to check if the plan is being used
        // in any active orders before allowing deletion. For now, we'll allow deletion.
        // You could add a check like:
        // if (existingPlan.Orders.Any(o => o.Status == "Active"))
        // {
        //     throw new BadRequestException("Cannot delete subscription plan that has active orders.");
        // }

        await subscriptionPlanRepo.DeleteAsync(id, ct);
    }
}