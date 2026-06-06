using Anonwork.Application.Common;
using Anonwork.Application.Common.Exceptions;
using Anonwork.Application.Common.Model;
using Anonwork.Application.Features.Auth.DTOs.Requests;
using Anonwork.Application.Interfaces;
using Anonwork.Domain.Entities;

namespace Anonwork.Application.Features.Auth;

public class VerifyEmailUseCase(
    IUnitOfWork unitOfWork,
    IJwtService jwtService)
{
    private readonly IGenericRepository<EmailVerificationToken> _tokenRepo = unitOfWork.GetRepository<EmailVerificationToken>();
    private readonly IGenericRepository<User> _userRepo = unitOfWork.GetRepository<User>();
    private readonly IGenericRepository<Role> _roleRepo = unitOfWork.GetRepository<Role>();
    private readonly IGenericRepository<UserRole> _userRoleRepo = unitOfWork.GetRepository<UserRole>();

    public async Task<AuthResult> ExecuteAsync(VerifyEmailRequest req, CancellationToken ct = default)
    {
        var email = req.Email.ToLower().Trim();
        var token = req.Token.Trim();

        if (token.Length != 6 || !token.All(char.IsDigit))
            throw new ConflictException("Verification code must be a 6-digit number.");

        var tokenHash = RegisterUseCase.HashToken(token);

        var verificationToken = await _tokenRepo.FindSingleWithTrackingAsync(
            t => t.Email == email && t.TokenHash == tokenHash && !t.IsUsed,
            ct);

        if (verificationToken is null)
            throw new ConflictException("Verification code is invalid.");

        if (verificationToken.ExpiresAt < DateTime.UtcNow)
            throw new ConflictException("Verification code has expired.");

        var user = await _userRepo.FindSingleWithTrackingAsync(u => u.Email == email, ct)
            ?? throw new ConflictException("User not found.");

        if (user.IsEmailVerified)
            throw new ConflictException("Email already verified.");

        var defaultRole = await _roleRepo.FindSingleAsync(r => r.Name == "user", ct)
            ?? throw new ConflictException("Default role 'user' was not found.");

        var alreadyAssigned = await _userRoleRepo.ExistsAsync(ur => ur.UserId == user.Id && ur.RoleId == defaultRole.Id, ct);
        if (!alreadyAssigned)
        {
            await _userRoleRepo.AddAsync(new UserRole
            {
                UserId = user.Id,
                RoleId = defaultRole.Id,
                CreatedAt = DateTime.UtcNow
            }, ct);
        }

        user.MarkEmailVerified();
        verificationToken.MarkVerified();
        await unitOfWork.SaveChangesAsync(ct);

        var permissions = Array.Empty<string>();
        //var accessToken = jwtService.GenerateAccessToken(user, permissions);
        //var refreshToken = await jwtService.GenerateRefreshTokenAsync(user.Id, ct);

        return new AuthResult("", "", user.Id, user.AnonAlias);
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
