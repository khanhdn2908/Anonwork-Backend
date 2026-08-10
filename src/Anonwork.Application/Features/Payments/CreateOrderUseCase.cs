using Anonwork.Application.Common.Exceptions;
using Anonwork.Application.Features.Payments.DTOs.Requests;
using Anonwork.Application.Features.Payments.DTOs.Responses;
using Anonwork.Application.Interfaces;
using Anonwork.Domain.Entities;
using Anonwork.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Anonwork.Application.Features.Payments;

public class CreateOrderUseCase(
    IUnitOfWork unitOfWork,
    ISepayService sepayService,
    IActivityLogService activityLogService)
{
    private readonly IGenericRepository<Order> _orderRepo = unitOfWork.GetRepository<Order>();
    private readonly IGenericRepository<SubscriptionPlan> _planRepo = unitOfWork.GetRepository<SubscriptionPlan>();
    private readonly IGenericRepository<UserSubscription> _subRepo = unitOfWork.GetRepository<UserSubscription>();
    private readonly IActivityLogService _activityLogService = activityLogService;

    public async Task<OrderResponse> ExecuteAsync(
    Guid userId,
    CreateOrderRequest req,
    CancellationToken ct = default)
    {
        var plan = await _planRepo.GetByIdAsync(req.PlanId, ct)
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

        await _orderRepo.AddAsync(order, ct);
        await unitOfWork.SaveChangesAsync(ct);

        _ = _activityLogService.LogAsync(
            userId,
            "CREATE_ORDER",
            "Payment",
            $"Tạo đơn hàng thanh toán gói '{plan.Name}' (Mã đơn: {order.OrderCode}, Số tiền: {order.Amount:N0} VND)",
            "order",
            order.Id.ToString(),
            ct: ct);

        return new OrderResponse(
            order.Id,
            order.OrderCode,
            transferContent,
            qrUrl,
            order.Amount,
            order.Status.ToString(),
            sepayService.GetAccountName(),
            sepayService.GetBankCode(),
            sepayService.GetBankAccount()
        );
    }

    private static string GenerateOrderCode()
    {
        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        var suffix = Guid.NewGuid().ToString("N")[..6].ToUpper();
        return $"ORD{timestamp}{suffix}";
    }
}
