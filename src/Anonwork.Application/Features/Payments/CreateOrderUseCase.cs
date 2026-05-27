using Anonwork.Application.Common.Exceptions;
using Anonwork.Application.Features.Payments.DTOs;
using Anonwork.Application.Interfaces;
using Anonwork.Domain.Common.Exceptions;
using Anonwork.Domain.Entities;
using Anonwork.Domain.Enums;

namespace Anonwork.Application.Features.Payments;

public class CreateOrderUseCase(
    IOrderRepository orderRepo,
    ISubscriptionPlanRepository planRepo,
    ISepayService sepayService)
{
    public async Task<OrderResponse> ExecuteAsync(
    Guid userId,
    CreateOrderRequest req,
    CancellationToken ct = default)
    {
        var plan = await planRepo.GetByIdAsync(req.PlanId, ct)
            ?? throw new NotFoundException("Subscription plan not found.");

        if (!plan.IsActive)
            throw new BadRequestException("This subscription plan is not available.");

        var now = DateTime.UtcNow;

        var orderId = Guid.NewGuid();

        var orderCode = GenerateOrderCode();

        var transferContent = sepayService.GenerateTransferContent(orderCode);

        var qrUrl = sepayService.GenerateQrUrl(
            plan.Price,
            transferContent);

        var order = new Order
        {
            Id = orderId,
            UserId = userId,
            PlanId = req.PlanId,
            OrderCode = orderCode,
            TransferContent = transferContent,
            Amount = plan.Price,
            Currency = "VND",
            Status = OrderStatus.Pending,
            PaymentMethod = PaymentMethod.BankTransfer,
            ExpiresAt = now.AddHours(24),
            CreatedAt = now,
            UpdatedAt = now
        };

        await orderRepo.CreateAsync(order, ct);

        return new OrderResponse(
            order.Id,
            order.OrderCode,
            transferContent,
            qrUrl,
            order.Amount,
            order.Status.ToString()
        );
    }

    private static string GenerateOrderCode()
    {
        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        var suffix = Guid.NewGuid().ToString("N")[..6].ToUpper();
        return $"ORD{timestamp}{suffix}";
    }
}
