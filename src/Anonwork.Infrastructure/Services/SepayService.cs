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
        string rawBody,
        string? timestamp,
        string? signatureHeader)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(signatureHeader))
                return false;

            var secretKey = !string.IsNullOrWhiteSpace(_options.ApiSecret)
                ? _options.ApiSecret
                : _options.ApiKey;

            if (string.IsNullOrWhiteSpace(secretKey))
            {
                _logger.LogWarning("Sepay ApiSecret is not configured. Webhook signature verification failed.");
                return false;
            }

            var signature = signatureHeader.Trim();
            if (signature.StartsWith("sha256=", StringComparison.OrdinalIgnoreCase))
            {
                signature = signature["sha256=".Length..].Trim();
            }

            if (long.TryParse(timestamp, out var ts))
            {
                var requestTime = DateTimeOffset.FromUnixTimeSeconds(ts);
                var timeDiff = Math.Abs((DateTimeOffset.UtcNow - requestTime).TotalMinutes);
                if (timeDiff > 5)
                {
                    _logger.LogWarning("Webhook request timestamp difference is too large: {Minutes} minutes", timeDiff);
                    return false;
                }
            }

            var dataToSign = string.IsNullOrEmpty(timestamp)
                ? rawBody
                : $"{timestamp}.{rawBody}";

            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secretKey));
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(dataToSign));
            var computedSignature = Convert.ToHexString(hash).ToLowerInvariant();

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

    public bool VerifyApiKey(string? authorizationHeader)
    {
        var expectedKey = !string.IsNullOrWhiteSpace(_options.ApiKey)
            ? _options.ApiKey
            : _options.ApiSecret;

        if (string.IsNullOrWhiteSpace(expectedKey))
        {
            _logger.LogWarning("Sepay ApiKey/ApiSecret is not configured in appsettings. Skipping webhook Authorization header verification.");
            return true;
        }

        if (string.IsNullOrWhiteSpace(authorizationHeader))
        {
            _logger.LogWarning("Webhook request is missing Authorization header.");
            return false;
        }

        var token = authorizationHeader.Trim();
        if (token.StartsWith("Apikey ", StringComparison.OrdinalIgnoreCase))
        {
            token = token["Apikey ".Length..].Trim();
        }
        else if (token.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
        {
            token = token["Bearer ".Length..].Trim();
        }

        return CryptographicOperations.FixedTimeEquals(
            Encoding.UTF8.GetBytes(token),
            Encoding.UTF8.GetBytes(expectedKey));
    }
}
