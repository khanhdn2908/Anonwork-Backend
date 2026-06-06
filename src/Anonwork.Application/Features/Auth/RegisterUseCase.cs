using System.Security.Cryptography;
using System.Text;
using Anonwork.Application.Common;
using Anonwork.Application.Common.Exceptions;
using Anonwork.Application.Features.Auth.DTOs.Requests;
using Anonwork.Application.Interfaces;
using Anonwork.Domain.Entities;

namespace Anonwork.Application.Features.Auth;

public class RegisterUseCase(
    IUnitOfWork unitOfWork,
    IPasswordHasher passwordHasher,
    IEmailSender emailSender)
{
    private readonly IGenericRepository<User> _userRepo = unitOfWork.GetRepository<User>();
    private readonly IGenericRepository<EmailVerificationToken> _verificationTokenRepo = unitOfWork.GetRepository<EmailVerificationToken>();

    public async Task ExecuteAsync(RegisterRequest req, CancellationToken ct = default)
    {
        var email = NormalizeEmail(req.Email);
        var username = NormalizeUsername(req.Username);
        var existingUser = await GetExistingUserAsync(email, ct);

        ValidateRegistration(existingUser, username, email, ct);

        var alias = await ResolveAnonAliasAsync(req.AnonAlias, ct, existingUser?.AnonAlias);
        var verification = CreateVerificationContext(email, username);

        await UpsertUserAsync(existingUser, req, username, email, alias, ct);
        await SaveVerificationTokenAsync(verification, ct);
        await unitOfWork.SaveChangesAsync(ct);
        await SendVerificationEmailAsync(email, username, verification.Token, ct);
    }

    private async Task<User?> GetExistingUserAsync(string email, CancellationToken ct)
        => await _userRepo.FindSingleAsync(u => u.Email == email, ct);

    private void ValidateRegistration(User? existingUser, string username, string email, CancellationToken ct)
    {
        if (existingUser is not null && existingUser.IsEmailVerified)
            throw new ConflictException("Email already in use.");
    }

    private async Task UpsertUserAsync(
        User? existingUser,
        RegisterRequest req,
        string username,
        string email,
        string alias,
        CancellationToken ct)
    {
        if (existingUser is null)
        {
            var user = User.Create(username, email, passwordHasher.Hash(req.Password), alias);
            await _userRepo.AddAsync(user, ct);
            return;
        }

        existingUser.Username = username;
        existingUser.PasswordHash = passwordHasher.Hash(req.Password);
        existingUser.AnonAlias = alias;
        existingUser.IsEmailVerified = false;
        existingUser.UpdatedAt = DateTime.UtcNow;
        await _userRepo.UpdateAsync(existingUser, ct);
    }

    private async Task SaveVerificationTokenAsync(VerificationContext verification, CancellationToken ct)
    {
        var verificationToken = EmailVerificationToken.Create(verification.Email, verification.Username, verification.TokenHash, verification.ExpiresAt);
        await _verificationTokenRepo.AddAsync(verificationToken, ct);
    }

    private async Task SendVerificationEmailAsync(string email, string username, string token, CancellationToken ct)
    {
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
    }

    private static string NormalizeEmail(string email)
        => email.ToLower().Trim();

    private static string NormalizeUsername(string username)
        => username.Trim();

    private static string GenerateVerificationToken()
    {
        var code = RandomNumberGenerator.GetInt32(0, 1_000_000);
        return code.ToString("D6");
    }

    private static VerificationContext CreateVerificationContext(string email, string username)
    {
        var token = GenerateVerificationToken();
        return new VerificationContext(
            email,
            username,
            token,
            HashToken(token),
            DateTime.UtcNow.AddMinutes(15));
    }

    public static string HashToken(string token)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(token.Trim().ToLowerInvariant()));
        return Convert.ToHexString(bytes).ToLowerInvariant();
    }

    private async Task<string> ResolveAnonAliasAsync(string? requested, CancellationToken ct, string? currentAlias = null)
    {
        if (requested is not null)
        {
            var normalizedRequested = requested.Trim();
            if (string.Equals(currentAlias, normalizedRequested, StringComparison.OrdinalIgnoreCase))
                return normalizedRequested;

            if (await _userRepo.ExistsAsync(u => u.AnonAlias == normalizedRequested, ct))
                throw new ConflictException("Anon alias already taken.");

            return normalizedRequested;
        }

        if (!string.IsNullOrWhiteSpace(currentAlias))
            return currentAlias;

        for (var i = 0; i < 5; i++)
        {
            var alias = AnonAliasGenerator.Generate();
            if (!await _userRepo.ExistsAsync(u => u.AnonAlias == alias, ct))
                return alias;
        }

        throw new InvalidOperationException("Failed to generate unique anon alias.");
    }

    private sealed record VerificationContext(
        string Email,
        string Username,
        string Token,
        string TokenHash,
        DateTime ExpiresAt);
}