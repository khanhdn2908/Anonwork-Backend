using Anonwork.Application.Common.Exceptions;
using Anonwork.Application.Features.Auth.DTOs.Requests;
using Anonwork.Application.Features.Auth.DTOs.Responses;
using Anonwork.Application.Interfaces;
using Anonwork.Domain.Entities;
using Anonwork.Domain.Enums;

namespace Anonwork.Application.Features.Auth;

public class LoginUseCase(
    IUnitOfWork unitOfWork,
    IJwtService jwtService,
    IPasswordHasher passwordHasher,
    IRolePermissionService rolePermissionService)
{
    private readonly IGenericRepository<User> _userRepo = unitOfWork.GetRepository<User>();

    public async Task<AuthResult> ExecuteAsync(LoginRequest req, CancellationToken ct = default)
    {
        const string invalidMsg = "Invalid email or password.";

        var user = await _userRepo.FindSingleAsync(u => u.Email == req.Email)
            ?? throw new UnauthorizedException(invalidMsg);

        if (user.Status == UserStatus.PendingVerification)
            throw new UnauthorizedException("Email has not been verified yet.");

        if (user.Status == UserStatus.Deleted)
            throw new UnauthorizedException("User account has been deleted.");

        if (user.Status == UserStatus.Suspended)
            throw new UnauthorizedException("User account has been suspended.");

        if (!passwordHasher.Verify(req.Password, user.PasswordHash))
            throw new UnauthorizedException(invalidMsg);

        var permissions = await rolePermissionService.GetPermissionCodesAsync(user.Id, ct);
        var accessToken = jwtService.GenerateAccessToken(user, permissions);
        var refreshToken = await jwtService.GenerateRefreshTokenAsync(user.Id, ct);

        return new AuthResult(accessToken, refreshToken, user.Id, user.AnonAlias);
    }
}
