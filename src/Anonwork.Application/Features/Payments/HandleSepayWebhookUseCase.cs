using Anonwork.Application.Common.Exceptions;
using Anonwork.Application.Features.Payments.DTOs.Requests;
using Anonwork.Application.Features.Payments.DTOs.Responses;
using Anonwork.Application.Interfaces;
using Anonwork.Domain.Entities;
using Anonwork.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace Anonwork.Application.Features.Payments;

public class HandleSepayWebhookUseCase(IUnitOfWork unitOfWork, IActivityLogService activityLogService, ILogger<HandleSepayWebhookUseCase> logger)
{
    private readonly IGenericRepository<Order> _orderRepository = unitOfWork.GetRepository<Order>();
    private readonly IGenericRepository<UserSubscription> _userSubscriptionRepository = unitOfWork.GetRepository<UserSubscription>();
    private readonly IGenericRepository<SubscriptionPlan> _subscriptionPlanRepository = unitOfWork.GetRepository<SubscriptionPlan>();
    private readonly IActivityLogService _activityLogService = activityLogService;
    private readonly ILogger<HandleSepayWebhookUseCase> _logger = logger;

    public async Task<WebhookResult> ExecuteAsync(SepayWebhookRequest request, CancellationToken ct = default)
    {
        _logger.LogInformation(
            "Received Sepay webhook. TransactionId={Id}, ReferenceCode={ReferenceCode}, Content={Content}",
            request.Id, request.ReferenceCode, request.Content);

        var orderCode = ExtractOrderCode(request.Content);
        if (orderCode is null)
        {
            _logger.LogWarning(
                "Cannot extract OrderCode from Content. TransactionId={Id}, Content={Content}",
                request.Id, request.Content);

            // Trả 200 để Sepay không retry — đây là giao dịch không liên quan đến hệ thống
            return WebhookResult.Ok("Transaction not related to any order");
        }

        // 2. Tìm order theo OrderCode
        var order = await _orderRepository.FindSingleAsync(o => o.OrderCode == orderCode);
        if (order is null)
        {
            _logger.LogWarning(
                "Order not found. OrderCode={OrderCode}, TransactionId={Id}",
                orderCode, request.Id);

            return WebhookResult.Ok("Order not found");
        }

        // 3. Idempotency: bỏ qua nếu order đã được xử lý
        if (order.Status is OrderStatus.Paid)
        {
            _logger.LogInformation(
                "Order already paid, skipping. OrderCode={OrderCode}, TransactionId={Id}",
                orderCode, request.Id);

            return WebhookResult.Ok();
        }

        // 4. Kiểm tra order còn hạn xử lý không
        if (order.ExpiresAt < DateTime.UtcNow)
        {
            _logger.LogWarning(
                "Order expired. OrderCode={OrderCode}, ExpiresAt={ExpiresAt}",
                orderCode, order.ExpiresAt);

            order.Status = OrderStatus.Expired;
            order.UpdatedAt = DateTime.UtcNow;
            await _orderRepository.UpdateAsync(order, ct);
            await unitOfWork.SaveChangesAsync(ct);

            return WebhookResult.Ok("Order expired");
        }

        // 5. Kiểm tra số tiền — Sepay gửi TransferAmount (VND)
        if (request.TransferAmount < order.Amount)
        {
            _logger.LogError(
                "Insufficient amount. OrderCode={OrderCode}, Expected={Expected}, Got={Got}",
                orderCode, order.Amount, request.TransferAmount);

            // Không reject (vẫn 200) nhưng không activate subscription
            return WebhookResult.Ok("Insufficient amount");
        }

        // 6. Kiểm tra chiều giao dịch — chỉ xử lý tiền vào (in)
        if (!string.Equals(request.TransferType, "in", StringComparison.OrdinalIgnoreCase))
        {
            _logger.LogInformation(
                "Ignoring non-credit transaction. TransferType={TransferType}, TransactionId={Id}",
                request.TransferType, request.Id);

            return WebhookResult.Ok("Not a credit transaction");
        }

        // 7. Kích hoạt subscription
        await ActivateSubscriptionAsync(order, request, ct);

        return WebhookResult.Ok();
    }

    // ─── Private Helpers ──────────────────────────────────────────────────────

    /// <summary>
    /// Parse OrderCode từ nội dung chuyển khoản.
    /// OrderCode có format "ORD-{yyyyMMdd}-{random}", ví dụ: ORD-20240527-ABC123
    /// </summary>
    private static string? ExtractOrderCode(string content)
    {
        if (string.IsNullOrWhiteSpace(content))
            return null;

        const string prefix = "ANON";

        var index = content.IndexOf(prefix, StringComparison.OrdinalIgnoreCase);
        if (index == -1)
            return null;

        // Bỏ prefix "ANON", lấy phần còn lại
        return content[(index + prefix.Length)..];
    }

    private async Task ActivateSubscriptionAsync(Order order, SepayWebhookRequest request, CancellationToken ct)
    {
        if (order.PlanId is null)
        {
            throw new InvalidOperationException(
                "Order does not contain PlanId.");
        }

        // Lấy subscription plan
        var plan = await _subscriptionPlanRepository.GetByIdAsync(order.PlanId.Value, ct);
        if (plan is null)
        {
            _logger.LogError("SubscriptionPlan not found. PlanId={PlanId}", order.PlanId);
            throw new InvalidOperationException($"SubscriptionPlan {order.PlanId} not found.");
        }

        // Cập nhật order → Paid
        order.Status = OrderStatus.Paid;
        order.ProviderTransactionId = request.ReferenceCode; // mã tham chiếu Sepay
        order.PaidAt = request.TransactionDate.ToUniversalTime();
        order.UpdatedAt = DateTime.UtcNow;
        await _orderRepository.UpdateAsync(order, ct);

        // Tạo hoặc gia hạn UserSubscription
        var existing = await _userSubscriptionRepository.FindSingleAsync(s => s.UserId == order.UserId && 
                                                                        s.Status == SubscriptionStatus.Active);

        if (existing is not null)
        {
            // Gia hạn: cộng thêm duration vào expires_at hiện tại (không mất ngày còn lại)
            var baseDate = existing.ExpiresAt > DateTime.UtcNow
                ? existing.ExpiresAt
                : DateTime.UtcNow;

            existing.ExpiresAt = baseDate.AddDays(plan.DurationDays);
            //existing.UpdatedAt = DateTime.UtcNow;
            await _userSubscriptionRepository.UpdateAsync(existing, ct);
            await unitOfWork.SaveChangesAsync(ct);

            _logger.LogInformation(
                "Subscription renewed. UserId={UserId}, NewExpiry={NewExpiry}",
                order.UserId, existing.ExpiresAt);
        }
        else
        {
            var subscription = new UserSubscription
            {
                Id = Guid.NewGuid(),
                UserId = order.UserId,
                PlanId = order.PlanId.Value,
                OrderId = order.Id,
                Status = SubscriptionStatus.Active,
                StartedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(plan.DurationDays),
                CreatedAt = DateTime.UtcNow,
                //UpdatedAt = DateTime.UtcNow,
            };

            await _userSubscriptionRepository.AddAsync(subscription, ct);
            await unitOfWork.SaveChangesAsync(ct);

            _logger.LogInformation(
                "Subscription created. UserId={UserId}, PlanId={PlanId}, ExpiresAt={ExpiresAt}",
                order.UserId, order.PlanId, subscription.ExpiresAt);
        }

        _ = _activityLogService.LogAsync(
            order.UserId,
            "PAYMENT_SUCCESS",
            "Payment",
            $"Thanh toán đơn hàng thành công cho gói '{plan.Name}' (Mã đơn: {order.OrderCode}, Số tiền: {order.Amount:N0} VND)",
            "order",
            order.Id.ToString(),
            ct: ct);
    }
}