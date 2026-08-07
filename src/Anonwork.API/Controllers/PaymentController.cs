using System.Text;
using System.Text.Json;
using Anonwork.Application.Features.Payments;
using Anonwork.Application.Features.Payments.DTOs.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Anonwork.Application.Interfaces;

namespace Anonwork.API.Controllers;

[Authorize]
[ApiController]
[Route("api/v1/payments")]
public class PaymentController(
    CreateOrderUseCase createOrderUseCase,
    GetOrderStatusUseCase getOrderStatusUseCase,
    HandleSepayWebhookUseCase handleSepayWebhookUseCase,
    RenewSubscriptionUseCase renewSubscriptionUseCase,
    ISepayService sepayService) : BaseApiController
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
    /// Webhook từ Sepay (Hỗ trợ xác thực HMAC-SHA256 qua x-SePay-Signature hoặc Authorization Header)
    /// </summary>
    [HttpPost("webhook")]
    [AllowAnonymous]
    public async Task<IActionResult> HandleWebhook(
        [FromHeader(Name = "Authorization")] string? authHeader,
        [FromHeader(Name = "x-SePay-Signature")] string? sepaySignature,
        [FromHeader(Name = "x-SePay-Timestamp")] string? sepayTimestamp,
        CancellationToken ct)
    {
        Request.EnableBuffering();
        Request.Body.Position = 0;
        using var reader = new StreamReader(Request.Body, Encoding.UTF8, leaveOpen: true);
        var rawBody = await reader.ReadToEndAsync(ct);
        Request.Body.Position = 0;

        bool isAuthenticated = false;

        if (!string.IsNullOrWhiteSpace(sepaySignature))
        {
            isAuthenticated = sepayService.VerifyWebhookSignature(rawBody, sepayTimestamp, sepaySignature);
        }
        else if (!string.IsNullOrWhiteSpace(authHeader))
        {
            isAuthenticated = sepayService.VerifyApiKey(authHeader);
        }

        if (!isAuthenticated)
        {
            return Unauthorized(new { message = "Invalid SePay webhook authentication / signature" });
        }

        SepayWebhookRequest? request;
        try
        {
            request = JsonSerializer.Deserialize<SepayWebhookRequest>(rawBody, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = "Invalid JSON payload format", error = ex.Message });
        }

        if (request is null)
        {
            return BadRequest(new { message = "Empty webhook payload" });
        }

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
