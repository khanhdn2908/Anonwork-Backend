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
    IRolePermissionService rolePermissionService,
    IActivityLogService activityLogService)
{
    private readonly IGenericRepository<User> _userRepo = unitOfWork.GetRepository<User>();
    private readonly IActivityLogService _activityLogService = activityLogService;

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
        var roles = await rolePermissionService.GetRoleCodesAsync(user.Id, ct);
        var accessToken = jwtService.GenerateAccessToken(user, permissions, roles);
        var refreshToken = await jwtService.GenerateRefreshTokenAsync(user.Id, ct);

        _ = _activityLogService.LogAsync(
            user.Id,
            "LOGIN_SUCCESS",
            "Auth",
            $"Người dùng '{user.Username}' đăng nhập thành công",
            "user",
            user.Id.ToString(),
            ct: ct);

        return new AuthResult(accessToken, refreshToken, user.Id, user.AnonAlias);
    }
}
