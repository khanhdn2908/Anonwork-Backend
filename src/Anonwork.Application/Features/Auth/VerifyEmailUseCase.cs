using Anonwork.Application.Common;
using Anonwork.Application.Common.Exceptions;
using Anonwork.Application.Common.Model;
using Anonwork.Application.Features.Auth.DTOs;
using Anonwork.Application.Interfaces;
using Anonwork.Domain.Entities;

namespace Anonwork.Application.Features.Auth;

public class VerifyEmailUseCase(
    IUnitOfWork unitOfWork,
    IJwtService jwtService,
    IPasswordHasher passwordHasher)
{
    private readonly IGenericRepository<EmailVerificationToken> _tokenRepo = unitOfWork.GetRepository<EmailVerificationToken>();
    private readonly IGenericRepository<User> _userRepo = unitOfWork.GetRepository<User>();

    public async Task<AuthResult> ExecuteAsync(VerifyEmailRequest req, CancellationToken ct = default)
    {
        var email = req.Email.ToLower().Trim();
        var token = req.Token.Trim();

        if (token.Length != 6 || !token.All(char.IsDigit))
            throw new ConflictException("Verification code must be a 6-digit number.");

        var tokenHash = RegisterUseCase.HashToken(token);

        var verificationToken = await _tokenRepo.FindSingleAsync(
            t => t.Email == email && t.TokenHash == tokenHash && !t.IsUsed,
            ct);

        if (verificationToken is null)
            throw new ConflictException("Verification code is invalid.");

        if (verificationToken.ExpiresAt < DateTime.UtcNow)
            throw new ConflictException("Verification code has expired.");

        var existingUser = await _userRepo.FindSingleAsync(u => u.Email == email, ct);
        if (existingUser is not null)
            throw new ConflictException("Email already verified.");

        var alias = await ResolveAnonAliasAsync(null, ct);

        var user = User.Create(
            verificationToken.Username,
            email,
            passwordHasher.Hash(Guid.NewGuid().ToString("N")),
            alias);
        user.MarkEmailVerified();

        await _userRepo.AddAsync(user, ct);
        verificationToken.MarkVerified();
        await unitOfWork.SaveChangesAsync(ct);

        var accessToken = jwtService.GenerateAccessToken(user);
        var refreshToken = await jwtService.GenerateRefreshTokenAsync(user.Id, ct);

        return new AuthResult(accessToken, refreshToken, user.Id, user.AnonAlias, user.Role);
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