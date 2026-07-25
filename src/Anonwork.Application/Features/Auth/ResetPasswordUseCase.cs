using Anonwork.Application.Common.Exceptions;
using Anonwork.Application.Features.Auth.DTOs.Requests;
using Anonwork.Application.Interfaces;
using Anonwork.Domain.Entities;
using Anonwork.Domain.Enums;

namespace Anonwork.Application.Features.Auth;

public class ResetPasswordUseCase(
    IUnitOfWork unitOfWork,
    IPasswordHasher passwordHasher,
    IActivityLogService activityLogService)
{
    private readonly IGenericRepository<User> _userRepo = unitOfWork.GetRepository<User>();
    private readonly IGenericRepository<OneTimeToken> _tokenRepo = unitOfWork.GetRepository<OneTimeToken>();
    private readonly IActivityLogService _activityLogService = activityLogService;

    public async Task ExecuteAsync(ResetPasswordRequest req, CancellationToken ct = default)
    {
        var email = NormalizeEmail(req.Email);
        var token = req.Token.Trim();

        if (token.Length != 6 || !token.All(char.IsDigit))
            throw new ConflictException("Reset code must be a 6-digit number.");

        if (string.IsNullOrWhiteSpace(req.NewPassword))
            throw new ConflictException("New password is required.");

        var tokenHash = RegisterUseCase.HashToken(token);

        var resetToken = await _tokenRepo.FindSingleWithTrackingAsync(
            t => t.Email == email
                 && t.TokenHash == tokenHash
                 && t.Purpose == TokenPurpose.ForgotPassword,
            ct);

        if (resetToken is null)
            throw new ConflictException("Reset code is invalid.");

        if (resetToken.IsUsed || resetToken.UsedAt is not null)
            throw new ConflictException("Reset code has already been used.");

        if (resetToken.IsExpired || resetToken.ExpiresAt < DateTime.UtcNow)
            throw new ConflictException("Reset code has expired.");

        var user = await _userRepo.FindSingleWithTrackingAsync(u => u.Email == email, ct)
            ?? throw new ConflictException("User not found.");

        user.PasswordHash = passwordHasher.Hash(req.NewPassword);
        user.UpdatedAt = DateTime.UtcNow;
        resetToken.MarkUsed();

        await unitOfWork.SaveChangesAsync(ct);

        _ = _activityLogService.LogAsync(
            user.Id,
            "RESET_PASSWORD_SUCCESS",
            "Auth",
            $"Đặt lại mật khẩu thành công cho tài khoản '{user.Username}'",
            "user",
            user.Id.ToString(),
            ct: ct);
    }

    private static string NormalizeEmail(string email)
        => email.ToLower().Trim();
}
