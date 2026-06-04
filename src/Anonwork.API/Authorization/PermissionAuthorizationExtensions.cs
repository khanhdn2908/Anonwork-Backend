using Microsoft.AspNetCore.Authorization;

namespace Anonwork.API.Authorization;

public static class PermissionAuthorizationExtensions
{
    public static AuthorizationPolicyBuilder RequirePermission(this AuthorizationPolicyBuilder builder, string permission)
        => builder.AddRequirements(new Anonwork.Application.Common.Authorization.PermissionRequirement(permission));
}
