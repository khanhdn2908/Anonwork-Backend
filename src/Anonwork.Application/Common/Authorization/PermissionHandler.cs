using System.Security.Claims;
using Anonwork.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace Anonwork.Application.Common.Authorization;

public sealed class PermissionHandler(IRolePermissionService permissionService) : AuthorizationHandler<PermissionRequirement>
{
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
    {
        var userIdValue = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? context.User.FindFirst("sub")?.Value;

        if (string.IsNullOrWhiteSpace(userIdValue) || !Guid.TryParse(userIdValue, out var userId))
        {
            return;
        }

        var permissions = await permissionService.GetPermissionCodesAsync(userId);
        if (permissions.Contains(requirement.Permission, StringComparer.OrdinalIgnoreCase))
        {
            context.Succeed(requirement);
        }
    }
}
