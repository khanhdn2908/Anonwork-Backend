using Anonwork.Application.Common;
using Anonwork.Application.Common.Exceptions;
using Anonwork.Application.Common.Model;
using Anonwork.Application.Features.Auth.DTOs;
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

        var user = await _userRepo.FindSingleAsync(u => u.GoogleSubject == googleSubject, ct)
            ?? await _userRepo.FindSingleAsync(u => u.Email == email, ct);

        if (user is null)
        {
            var alias = await ResolveAnonAliasAsync(req.AnonAlias, ct);
            user = User.CreateGoogleUser(username, email, googleSubject, picture ?? string.Empty, alias);
            await _userRepo.AddAsync(user, ct);
        }
        else
        {
            user.LinkGoogleAccount(googleSubject, picture);

            if (string.IsNullOrWhiteSpace(user.AvatarUrl) && !string.IsNullOrWhiteSpace(picture))
            {
                user.AvatarUrl = picture;
            }
        }

        await unitOfWork.SaveChangesAsync(ct);

        var accessToken = jwtService.GenerateAccessToken(user);
        var refreshToken = await jwtService.GenerateRefreshTokenAsync(user.Id, ct);

        return new AuthResult(accessToken, refreshToken, user.Id, user.AnonAlias, user.Role);
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
