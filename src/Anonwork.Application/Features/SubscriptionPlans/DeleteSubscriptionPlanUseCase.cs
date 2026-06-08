using Anonwork.Application.Common.Exceptions;
using Anonwork.Application.Interfaces;
using Anonwork.Domain.Common.Exceptions;
using Anonwork.Domain.Entities;

namespace Anonwork.Application.Features.SubscriptionPlans;

public class DeleteSubscriptionPlanUseCase(IUnitOfWork unitOfWork)
{
    private readonly IGenericRepository<SubscriptionPlan> _subscriptionPlanRepo = unitOfWork.GetRepository<SubscriptionPlan>();

    public async Task ExecuteAsync(Guid id, CancellationToken ct = default)
    {
        // Check if plan exists
        var existingPlan = await _subscriptionPlanRepo.GetByIdWithTrackingAsync(id, ct)
            ?? throw new NotFoundException($"Subscription plan with ID {id} not found.");

        existingPlan.IsActive = false;
        //await _subscriptionPlanRepo.DeleteAsync(id, ct);
        await unitOfWork.SaveChangesAsync(ct);
    }
}