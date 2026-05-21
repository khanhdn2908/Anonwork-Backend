using Anonwork.Application.Common;
using Anonwork.Application.Common.Exceptions;
using Anonwork.Application.Interfaces;
using Anonwork.Domain.Entities;

namespace Anonwork.Application.UseCases.Auth;

public class RegisterUseCase(
    IUserRepository userRepo,
    IJwtService jwtService,
    IPasswordHasher passwordHasher)
{
    public async Task<AuthResult> ExecuteAsync(RegisterRequest req, CancellationToken ct = default)
    {
        // ── Validation ──────────────────────────────
        if (await userRepo.ExistsByEmailAsync(req.Email, ct))
            throw new ConflictException("Email already in use.");

        if (await userRepo.ExistsByUsernameAsync(req.Username, ct))
            throw new ConflictException("Username already taken.");

        // ── Anon alias ──────────────────────────────
        var alias = await ResolveAnonAliasAsync(req.AnonAlias, ct);

        // ── Create user ─────────────────────────────
        var user = User.Create(
            req.Username,
            req.Email,
            passwordHasher.Hash(req.Password),
            alias);

        await userRepo.CreateAsync(user, ct);

        // ── Issue tokens ────────────────────────────
        var accessToken = jwtService.GenerateAccessToken(user);
        var refreshToken = await jwtService.GenerateRefreshTokenAsync(user.Id, ct);

        return new AuthResult(accessToken, refreshToken, user.Id, user.AnonAlias, user.Role);
    }

    private async Task<string> ResolveAnonAliasAsync(string? requested, CancellationToken ct)
    {
        if (requested is not null)
        {
            if (await userRepo.ExistsByAnonAliasAsync(requested, ct))
                throw new ConflictException("Anon alias already taken.");
            return requested;
        }

        // Auto-generate với retry tối đa 5 lần
        for (var i = 0; i < 5; i++)
        {
            var alias = AnonAliasGenerator.Generate();
            if (!await userRepo.ExistsByAnonAliasAsync(alias, ct))
                return alias;
        }

        throw new InvalidOperationException("Failed to generate unique anon alias.");
    }
}