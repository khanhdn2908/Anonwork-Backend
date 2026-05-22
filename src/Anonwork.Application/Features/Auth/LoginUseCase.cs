using Anonwork.Application.Common.Exceptions;
using Anonwork.Application.Common.Model;
using Anonwork.Application.Features.Auth.DTOs;
using Anonwork.Application.Interfaces;


namespace Anonwork.Application.Features.Auth;

public class LoginUseCase(
    IUserRepository userRepo,
    IJwtService jwtService,
    IPasswordHasher passwordHasher)
{
    public async Task<AuthResult> ExecuteAsync(LoginRequest req, CancellationToken ct = default)
    {
        // Luôn dùng message chung để tránh user enumeration
        const string invalidMsg = "Invalid email or password.";

        var user = await userRepo.GetByEmailAsync(req.Email, ct)
            ?? throw new UnauthorizedException(invalidMsg);

        if (!passwordHasher.Verify(req.Password, user.PasswordHash))
            throw new UnauthorizedException(invalidMsg);

        var accessToken = jwtService.GenerateAccessToken(user);
        var refreshToken = await jwtService.GenerateRefreshTokenAsync(user.Id, ct);

        return new AuthResult(accessToken, refreshToken, user.Id, user.AnonAlias, user.Role);
    }
}