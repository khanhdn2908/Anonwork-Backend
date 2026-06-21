using Anonwork.Application.Features.Payments;
using Anonwork.Application.Features.Payments.DTOs.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Anonwork.API.Controllers;

[Authorize]
[ApiController]
[Route("api/v1/payments")]
public class PaymentController(
    CreateOrderUseCase createOrderUseCase,
    GetOrderStatusUseCase getOrderStatusUseCase,
    HandleSepayWebhookUseCase handleSepayWebhookUseCase,
    RenewSubscriptionUseCase renewSubscriptionUseCase) : BaseApiController
{

    [HttpPost("create-order")]
    public async Task<IActionResult> CreateOrder(
        [FromBody] CreateOrderRequest req,
        CancellationToken ct)
    {
        var userId = GetUserIdFromToken();
        if (userId is null) return Unauthorized();

        var result = await createOrderUseCase.ExecuteAsync(userId.Value, req, ct);
        return CreatedAtAction(nameof(GetOrderStatus), new { orderId = result.Id }, result);
    }

    /// <summary>
    /// Lấy trạng thái order
    /// </summary>
    [HttpGet("orders/{orderId}")]
    public async Task<IActionResult> GetOrderStatus(
        Guid orderId,
        CancellationToken ct)
    {
        var userId = GetUserIdFromToken();
        if (userId is null)
            return Unauthorized(new { message = "User not authenticated" });

        var result = await getOrderStatusUseCase.ExecuteAsync(userId.Value, orderId, ct);
        return Ok(result);
    }

    /// <summary>
    /// Webhook từ Sepay (không cần authorization)
    /// </summary>
    [HttpPost("webhook")]
    [AllowAnonymous]
    public async Task<IActionResult> HandleWebhook(
        [FromBody] SepayWebhookRequest request,
        CancellationToken ct)
    {
        await handleSepayWebhookUseCase.ExecuteAsync(request, ct);
        return Ok(new { success = true });
    }

    /// <summary>
    /// Renew subscription
    /// </summary>
    [HttpPost("subscriptions/{subscriptionId}/renew")]
    public async Task<IActionResult> RenewSubscription(
        Guid subscriptionId,
        CancellationToken ct)
    {
        await renewSubscriptionUseCase.ExecuteAsync(subscriptionId, ct);
        return Ok(new { success = true });
    }
}
