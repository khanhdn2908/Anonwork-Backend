using Anonwork.Application.Features.Payments.DTOs;
using Anonwork.Application.Interfaces;
using Anonwork.Infrastructure.Common;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace Anonwork.Infrastructure.Services;

public class SepayService : ISepayService
{
    private readonly SepayOptions _options;
    private readonly ILogger<SepayService> _logger;

    public SepayService(
        IOptions<SepayOptions> options,
        ILogger<SepayService> logger)
    {
        _options = options.Value;
        _logger = logger;
    }

    public string GenerateTransferContent(string orderCode)
    {
        return $"ANON{orderCode}";
    }

    public string GenerateQrUrl(
       long amount,
       string transferContent)
    {
        return
            $"https://qr.sepay.vn/img" +
            $"?acc={_options.BankAccount}" +
            $"&bank={_options.BankCode}" +
            $"&amount={amount}" +
            $"&des={Uri.EscapeDataString(transferContent)}";
    }

    public string GetBankAccount() => _options.BankAccount;

    public string GetBankCode() => _options.BankCode;

    public string GetAccountName() => _options.AccountName;

    public bool VerifyWebhookSignature(
        string payload,
        string signature)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(signature))
                return false;

            using var hmac = new HMACSHA256(
                Encoding.UTF8.GetBytes(_options.ApiSecret));

            var hash = hmac.ComputeHash(
                Encoding.UTF8.GetBytes(payload));

            var computedSignature =
                Convert.ToHexString(hash).ToLowerInvariant();

            return CryptographicOperations.FixedTimeEquals(
                Encoding.UTF8.GetBytes(computedSignature),
                Encoding.UTF8.GetBytes(signature.ToLowerInvariant()));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Webhook signature verification failed");
            return false;
        }
    }
}
