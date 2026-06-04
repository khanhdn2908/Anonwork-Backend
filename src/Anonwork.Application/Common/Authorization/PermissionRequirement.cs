using Microsoft.AspNetCore.Authorization;

namespace Anonwork.Application.Common.Authorization;

public sealed class PermissionRequirement(string permission) : IAuthorizationRequirement
{
    public string Permission { get; } = permission;
}
