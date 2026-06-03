using Anonwork.Application.Interfaces;
using Anonwork.Infrastructure.Common;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Resend;

namespace Anonwork.Infrastructure.Services;

public class EmailSender : IEmailSender
{
    private readonly EmailOptions _options;
    private readonly ILogger<EmailSender> _logger;
    private readonly IResend _resend;

    public EmailSender(
        IOptions<EmailOptions> options,
        IOptionsSnapshot<ResendClientOptions> resendOptions,
        HttpClient httpClient,
        ILogger<EmailSender> logger)
    {
        _options = options.Value;
        _logger = logger;

        if (string.IsNullOrWhiteSpace(_options.ResendApiKey))
            throw new InvalidOperationException("Email:ResendApiKey is not configured.");

        var configuredResendOptions = resendOptions.Value;
        if (string.IsNullOrWhiteSpace(configuredResendOptions.ApiToken))
        {
            configuredResendOptions.ApiToken = _options.ResendApiKey;
        }

        _resend = new ResendClient(resendOptions, httpClient);
    }

    public async Task SendAsync(string to, string subject, string htmlBody, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(_options.FromAddress))
            throw new InvalidOperationException("Email:FromAddress is not configured.");

        var from = string.IsNullOrWhiteSpace(_options.FromName)
            ? _options.FromAddress
            : $"{_options.FromName} <{_options.FromAddress}>";

        var message = new EmailMessage
        {
            From = from,
            To = [to],
            Subject = subject,
            HtmlBody = htmlBody
        };

        try
        {
            var result = await _resend.EmailSendAsync(message, ct);

            if (result is null || !result.Success)
            {
                _logger.LogError(
                    "Failed to send email via Resend. To: {To}, Subject: {Subject}, From: {From}, Response: {Response}",
                    to,
                    subject,
                    from,
                    result);

                throw new InvalidOperationException("Failed to send email via Resend.");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Failed to send email via Resend. To: {To}, Subject: {Subject}, From: {From}",
                to,
                subject,
                from);

            throw;
        }
    }
}
