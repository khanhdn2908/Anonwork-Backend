using Anonwork.Application.Common.Exceptions;
using Anonwork.Application.Features.Payments.DTOs;
using Anonwork.Application.Interfaces;
using Anonwork.Domain.Common.Exceptions;
using Anonwork.Domain.Entities;
using Anonwork.Domain.Enums;

namespace Anonwork.Application.Features.Payments;

public class GetOrderStatusUseCase(IOrderRepository orderRepo, ISepayService sepayService)
{
    public async Task<OrderResponse> ExecuteAsync(
        Guid userId,
        Guid orderId,
        CancellationToken ct = default)
    {
        var order = await orderRepo.GetByIdAsync(orderId, ct)
            ?? throw new NotFoundException("Order not found.");

        if (order.UserId != userId) throw new UnauthorizedException("Order does not belong to you");

        if (order.Status == OrderStatus.Pending &&
           order.ExpiresAt is not null &&
           order.ExpiresAt < DateTime.UtcNow)
        {
            order.Status = OrderStatus.Expired;

            await orderRepo.UpdateAsync(order, ct);
        }

        var qrUrl = sepayService.GenerateQrUrl(
            order.Amount,
            order.TransferContent);

        return MapToResponse(order, qrUrl);
    }

    private static OrderResponse MapToResponse(Order order, string qrUrl) =>
        new(
           order.Id,
            order.OrderCode,
            order.TransferContent,
            qrUrl,
            order.Amount,
            order.Status.ToString()
        );
}
