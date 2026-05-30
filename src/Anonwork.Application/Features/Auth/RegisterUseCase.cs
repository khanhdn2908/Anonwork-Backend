using Anonwork.Application.Common;
using Anonwork.Application.Common.Exceptions;
using Anonwork.Application.Interfaces;
using Anonwork.Application.Features.Auth.DTOs;
using Anonwork.Domain.Entities;
using Anonwork.Application.Common.Model;

namespace Anonwork.Application.Features.Auth;

public class RegisterUseCase(IUnitOfWork unitOfWork, IJwtService jwtService, IPasswordHasher passwordHasher)
{
    private readonly IGenericRepository<User> _userRepo = unitOfWork.GetRepository<User>();

    public async Task<AuthResult> ExecuteAsync(RegisterRequest req, CancellationToken ct = default)
    {
        // ── Validation ──────────────────────────────
        if (await _userRepo.ExistsAsync(u => u.Email == req.Email.ToLower().Trim()))
            throw new ConflictException("Email already in use.");

        if (await _userRepo.ExistsAsync(u => u.Username == req.Username.ToLower().Trim()))
            throw new ConflictException("Username already taken.");

        // ── Anon alias ──────────────────────────────
        var alias = await ResolveAnonAliasAsync(req.AnonAlias, ct);

        // ── Create user ─────────────────────────────
        var user = User.Create(
            req.Username,
            req.Email,
            passwordHasher.Hash(req.Password),
            alias);

        await  _userRepo.AddAsync(user, ct);
        await unitOfWork.SaveChangesAsync();

        // ── Issue tokens ────────────────────────────
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

        // Auto-generate với retry tối đa 5 lần
        for (var i = 0; i < 5; i++)
        {
            var alias = AnonAliasGenerator.Generate();
            if (!await _userRepo.ExistsAsync(u => u.AnonAlias == alias, ct))
                return alias;
        }

        throw new InvalidOperationException("Failed to generate unique anon alias.");
    }
}