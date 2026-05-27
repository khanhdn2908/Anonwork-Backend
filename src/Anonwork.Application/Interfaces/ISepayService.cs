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
    /// Verify Sepay webhook signature
    /// </summary>
    bool VerifyWebhookSignature(
        string payload,
        string signature);
}
