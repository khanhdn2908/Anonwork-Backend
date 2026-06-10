using Anonwork.Application.Common.Exceptions;
using Anonwork.Application.Features.Auth.DTOs.Requests;
using Anonwork.Application.Interfaces;
using Anonwork.Domain.Entities;
using Anonwork.Domain.Enums;

namespace Anonwork.Application.Features.Auth;

public class ForgotPasswordUseCase(
    IUnitOfWork unitOfWork,
    IEmailSender emailSender)
{
    private readonly IGenericRepository<User> _userRepo = unitOfWork.GetRepository<User>();
    private readonly IGenericRepository<OneTimeToken> _tokenRepo = unitOfWork.GetRepository<OneTimeToken>();

    public async Task ExecuteAsync(ForgotPasswordRequest req, CancellationToken ct = default)
    {
        var email = NormalizeEmail(req.Email);
        var user = await _userRepo.FindSingleAsync(u => u.Email == email, ct);

        if (user is null)
            return;

        var existingTokens = await _tokenRepo.FindAsync(
            t => t.Email == email
                 && t.Purpose == TokenPurpose.ForgotPassword
                 && t.UsedAt == null,
            ct);

        foreach (var token in existingTokens)
        {
            token.MarkUsed();
            await _tokenRepo.UpdateAsync(token, ct);
        }

        var tokenValue = GenerateVerificationToken();
        var tokenHash = RegisterUseCase.HashToken(tokenValue);
        var expiresAt = DateTime.UtcNow.AddMinutes(15);

        var resetToken = OneTimeToken.Create(
            email: email,
            tokenHash: tokenHash,
            purpose: TokenPurpose.ForgotPassword,
            expiresAt: expiresAt,
            username: user.Username);

        await _tokenRepo.AddAsync(resetToken, ct);
        await unitOfWork.SaveChangesAsync(ct);

        await SendResetPasswordEmailAsync(user.Email, user.Username, tokenValue, ct);
    }

    private async Task SendResetPasswordEmailAsync(string email, string username, string token, CancellationToken ct)
    {
        var subject = "Reset your password for Anonwork";
        var body = $@"
            <div style=""font-family: Arial, sans-serif; line-height: 1.6; color: #1f2937;"">
                <p>Xin chào <strong>{username}</strong>,</p>

                <p>Chúng tôi nhận được yêu cầu đặt lại mật khẩu cho tài khoản Anonwork của bạn.</p>

                <div style=""margin: 24px 0; padding: 16px; border: 1px solid #e5e7eb; border-radius: 8px; background-color: #f9fafb; text-align: center;"">
                    <p style=""margin: 0 0 8px; font-size: 14px; color: #6b7280;"">Mã đặt lại mật khẩu của bạn là</p>
                    <div style=""font-size: 28px; font-weight: bold; letter-spacing: 4px; color: #111827;"">{token}</div>
                </div>

                <p>Mã này sẽ hết hạn sau <strong>15 phút</strong>. Nếu bạn không yêu cầu đặt lại mật khẩu, vui lòng bỏ qua email này.</p>

                <p style=""margin-top: 24px;"">Trân trọng,<br/>Đội ngũ Anonwork</p>
            </div>";

        await emailSender.SendAsync(email, subject, body, ct);
    }

    private static string NormalizeEmail(string email)
        => email.ToLower().Trim();

    private static string GenerateVerificationToken()
    {
        var code = Random.Shared.Next(0, 1_000_000);
        return code.ToString("D6");
    }
}
