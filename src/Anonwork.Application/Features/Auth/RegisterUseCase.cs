using System.Security.Cryptography;
using System.Text;
using Anonwork.Application.Common;
using Anonwork.Application.Common.Exceptions;
using Anonwork.Application.Features.Auth.DTOs;
using Anonwork.Application.Interfaces;
using Anonwork.Domain.Entities;

namespace Anonwork.Application.Features.Auth;

public class RegisterUseCase(
    IUnitOfWork unitOfWork,
    IPasswordHasher passwordHasher,
    IEmailSender emailSender)
{
    private readonly IGenericRepository<User> _userRepo = unitOfWork.GetRepository<User>();

    public async Task ExecuteAsync(RegisterRequest req, CancellationToken ct = default)
    {
        var email = req.Email.ToLower().Trim();
        var username = req.Username.Trim();

        if (await _userRepo.ExistsAsync(u => u.Email == email, ct))
            throw new ConflictException("Email already in use.");

        if (await _userRepo.ExistsAsync(u => u.Username == username.ToLower(), ct))
            throw new ConflictException("Username already taken.");

        var alias = await ResolveAnonAliasAsync(req.AnonAlias, ct);
        var token = GenerateVerificationToken();
        var tokenHash = HashToken(token);
        var expiresAt = DateTime.UtcNow.AddMinutes(15);

        var verificationToken = EmailVerificationToken.Create(email, username, tokenHash, expiresAt);
        await unitOfWork.GetRepository<EmailVerificationToken>().AddAsync(verificationToken, ct);
        await unitOfWork.SaveChangesAsync(ct);

        var subject = "Verify your email for Anonwork";
        var body = $@"
            <div style=""font-family: Arial, sans-serif; line-height: 1.6; color: #1f2937;"">
                <p>Xin chào <strong>{username}</strong>,</p>

                <p>Chúng tôi đã nhận được yêu cầu xác minh email cho tài khoản Anonwork của bạn.</p>

                <div style=""margin: 24px 0; padding: 16px; border: 1px solid #e5e7eb; border-radius: 8px; background-color: #f9fafb; text-align: center;"">
                    <p style=""margin: 0 0 8px; font-size: 14px; color: #6b7280;"">Mã xác minh của bạn là</p>
                    <div style=""font-size: 28px; font-weight: bold; letter-spacing: 4px; color: #111827;"">{token}</div>
                </div>

                <p>Mã này sẽ hết hạn sau <strong>15 phút</strong>. Nếu bạn không thực hiện yêu cầu này, vui lòng bỏ qua email này.</p>

                <p style=""margin-top: 24px;"">Trân trọng,<br/>Đội ngũ Anonwork</p>
            </div>";
        await emailSender.SendAsync(email, subject, body, ct);

        _ = alias;
    }

    private static string GenerateVerificationToken()
    {
        var code = RandomNumberGenerator.GetInt32(0, 1_000_000);
        return code.ToString("D6");
    }

    public static string HashToken(string token)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(token.Trim().ToLowerInvariant()));
        return Convert.ToHexString(bytes).ToLowerInvariant();
    }

    private async Task<string> ResolveAnonAliasAsync(string? requested, CancellationToken ct)
    {
        if (requested is not null)
        {
            if (await _userRepo.ExistsAsync(u => u.AnonAlias == requested, ct))
                throw new ConflictException("Anon alias already taken.");
            return requested;
        }

        for (var i = 0; i < 5; i++)
        {
            var alias = AnonAliasGenerator.Generate();
            if (!await _userRepo.ExistsAsync(u => u.AnonAlias == alias, ct))
                return alias;
        }

        throw new InvalidOperationException("Failed to generate unique anon alias.");
    }
}