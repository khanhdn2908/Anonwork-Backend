using Anonwork.Application.Common;
using Anonwork.Application.Common.Exceptions;
using Anonwork.Application.Features.Auth.DTOs.Requests;
using Anonwork.Application.Features.Auth.DTOs.Responses;
using Anonwork.Application.Interfaces;
using Anonwork.Domain.Entities;
using Google.Apis.Auth;
using Microsoft.Extensions.Configuration;

namespace Anonwork.Application.Features.Auth;

public class GoogleLoginUseCase(
    IUnitOfWork unitOfWork,
    IJwtService jwtService,
    IConfiguration configuration)
{
    private readonly IGenericRepository<User> _userRepo = unitOfWork.GetRepository<User>();
    private readonly IGenericRepository<Role> _roleRepo = unitOfWork.GetRepository<Role>();
    private readonly IGenericRepository<UserRole> _userRoleRepo = unitOfWork.GetRepository<UserRole>();

    public async Task<AuthResult> ExecuteAsync(GoogleLoginRequest req, CancellationToken ct = default)
    {
        var payload = await ValidateGoogleTokenAsync(req.IdToken, configuration, ct);

        if (!payload.EmailVerified)
            throw new UnauthorizedException("Google email is not verified.");

        var email = payload.Email?.ToLower().Trim();
        if (string.IsNullOrWhiteSpace(email))
            throw new UnauthorizedException("Google account does not contain an email.");

        var googleSubject = payload.Subject;
        var username = BuildUsername(payload.Name, email);
        var picture = payload.Picture;

        var user = await _userRepo.FindSingleWithTrackingAsync(u => u.GoogleSubject == googleSubject, ct)
            ?? await _userRepo.FindSingleWithTrackingAsync(u => u.Email == email, ct);

        var isNewUser = false;

        if (user is null)
        {
            var alias = await ResolveAnonAliasAsync(req.AnonAlias, ct);
            user = User.CreateGoogleUser(username, email, googleSubject, picture ?? string.Empty, alias);
            await _userRepo.AddAsync(user, ct);
            isNewUser = true;
        }
        else
        {
            user.LinkGoogleAccount(googleSubject, picture);

            if (string.IsNullOrWhiteSpace(user.AvatarUrl) && !string.IsNullOrWhiteSpace(picture))
            {
                user.AvatarUrl = picture;
            }
        }

        if (isNewUser)
        {
            await AssignDefaultUserRoleAsync(user.Id, ct);
        }

        await unitOfWork.SaveChangesAsync(ct);

        var permissions = Array.Empty<string>();
        var accessToken = jwtService.GenerateAccessToken(user, permissions);
        var refreshToken = await jwtService.GenerateRefreshTokenAsync(user.Id, ct);

        return new AuthResult(accessToken, refreshToken, user.Id, user.AnonAlias);
    }

    private static async Task<GoogleJsonWebSignature.Payload> ValidateGoogleTokenAsync(
        string idToken,
        IConfiguration configuration,
        CancellationToken ct)
    {
        var clientId = configuration["Google:ClientId"];
        if (string.IsNullOrWhiteSpace(clientId))
            throw new InvalidOperationException("Google:ClientId is not configured.");

        var settings = new GoogleJsonWebSignature.ValidationSettings
        {
            Audience = [clientId]
        };

        return await GoogleJsonWebSignature.ValidateAsync(idToken, settings);
    }

    private async Task AssignDefaultUserRoleAsync(Guid userId, CancellationToken ct)
    {
        var role = await _roleRepo.FindSingleAsync(r => r.Name == "user", ct)
            ?? throw new InvalidOperationException("Default role 'user' was not found.");

        var existing = await _userRoleRepo.ExistsAsync(ur => ur.UserId == userId && ur.RoleId == role.Id, ct);
        if (existing)
            return;

        await _userRoleRepo.AddAsync(new UserRole
        {
            UserId = userId,
            RoleId = role.Id,
            CreatedAt = DateTime.UtcNow
        }, ct);
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

    private static string BuildUsername(string? name, string email)
    {
        var baseName = !string.IsNullOrWhiteSpace(name)
            ? name
            : email.Split('@', 2)[0];

        var normalized = new string(baseName.Where(char.IsLetterOrDigit).ToArray());
        if (string.IsNullOrWhiteSpace(normalized))
        {
            normalized = "googleuser";
        }

        return normalized.Length > 50 ? normalized[..50] : normalized;
    }
}
