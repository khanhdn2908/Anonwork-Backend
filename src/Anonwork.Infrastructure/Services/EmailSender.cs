using System.Net;
using System.Net.Mail;
using Anonwork.Application.Interfaces;
using Anonwork.Infrastructure.Common;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Anonwork.Infrastructure.Services;

public class EmailSender(
    IOptions<EmailOptions> options,
    ILogger<EmailSender> logger) : IEmailSender
{
    private readonly EmailOptions _options = options.Value;
    private readonly ILogger<EmailSender> _logger = logger;

    public async Task SendAsync(string to, string subject, string htmlBody, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(_options.Host))
            throw new InvalidOperationException("Email:Host is not configured.");

        using var message = new MailMessage
        {
            From = new MailAddress(_options.FromAddress, _options.FromName),
            Subject = subject,
            Body = htmlBody,
            IsBodyHtml = true
        };

        message.To.Add(to);

        using var client = new SmtpClient(_options.Host, _options.Port)
        {
            EnableSsl = _options.EnableSsl,
            Credentials = new NetworkCredential(_options.Username, _options.Password)
        };

        try
        {
            await client.SendMailAsync(message, ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Failed to send email. To: {To}, Subject: {Subject}, Host: {Host}, Port: {Port}, Ssl: {Ssl}, Username: {Username}",
                to,
                subject,
                _options.Host,
                _options.Port,
                _options.EnableSsl,
                _options.Username);

            throw;
        }
    }
}