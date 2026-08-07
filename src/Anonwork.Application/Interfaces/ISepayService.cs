using Anonwork.Application.Features.Payments.DTOs;

namespace Anonwork.Application.Interfaces;

public interface ISepayService
{
    /// <summary>
    /// Generate unique transfer content
    /// </summary>
    string GenerateTransferContent(string orderCode);

    /// <summary>
    /// Generate Sepay QR image URL
    /// </summary>
    string GenerateQrUrl(
        long amount,
        string transferContent);

    /// <summary>
    /// Get configured bank account number
    /// </summary>
    string GetBankAccount();

    /// <summary>
    /// Get configured bank code
    /// </summary>
    string GetBankCode();

    /// <summary>
    /// Get configured account holder name
    /// </summary>
    string GetAccountName();

    /// <summary>
    /// Verify Sepay webhook signature (HMAC-SHA256)
    /// </summary>
    bool VerifyWebhookSignature(
        string rawBody,
        string? timestamp,
        string? signatureHeader);

    /// <summary>
    /// Verify Webhook Authorization header against configured ApiKey / ApiSecret
    /// </summary>
    bool VerifyApiKey(string? authorizationHeader);
}
