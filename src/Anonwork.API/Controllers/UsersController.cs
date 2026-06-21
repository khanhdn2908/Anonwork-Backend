using Anonwork.Application.Features.Users;
using Anonwork.Application.Features.Users.DTOs.Requests;
using Anonwork.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Anonwork.API.Controllers;

[ApiController]
[Route("api/v1/users")]
[Authorize]
public class UsersController(
    GetMeUseCase getMeUseCase,
    GetUserUseCase getUserUseCase,
    UpdateUserUseCase updateUserUseCase,
    ToggleUserAnonDefaultUseCase toggleUserAnonDefaultUseCase,
    AssignAnonImageToUserUseCase assignAnonImageToUserUseCase,
    DeleteUserUseCase deleteUserUseCase,
    DeleteUserUseCasePermanent deleteUserUseCasePermanent,
    GetAllUsersUseCase getAllUsersUseCase,
    AssignRoleToUserUseCase assignRoleToUserUseCase,
    RemoveRoleFromUserUseCase removeRoleFromUserUseCase,
    GetUserRolesUseCase getUserRolesUseCase,
    IAuthorizationService authorizationService) : BaseApiController
{
    [HttpGet("me")]
    public async Task<IActionResult> GetMe(CancellationToken ct)
    {
        var userId = GetUserIdFromToken();
        if (userId is null) return Unauthorized();

        var result = await getMeUseCase.ExecuteAsync(userId.Value, ct);
        return Ok(result);
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetUserById(Guid id, CancellationToken ct)
    {
        var authResult = await authorizationService.AuthorizeAsync(User, "Permission:users.read:all");

        var result = await getUserUseCase.ExecuteAsync(id, authResult.Succeeded,ct);
        return Ok(result);
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAllUsers(
        [FromQuery] string? search = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken ct = default)
    {
        var authResult = await authorizationService.AuthorizeAsync(User, "Permission:users.read:all");

        var result = await getAllUsersUseCase.ExecuteAsync(authResult.Succeeded, search, page, pageSize, ct);
        return Ok(result);
    }

    [HttpPut("me")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> UpdateMe(
        [FromForm] UpdateUserRequestDto req,
        CancellationToken ct)
    {
        var userId = GetUserIdFromToken();
        if (userId is null) return Unauthorized();

        var result = await updateUserUseCase.ExecuteAsync(userId.Value, req, ct);
        return Ok(result);
    }

    [HttpPatch("me/anon")]
    public async Task<IActionResult> ToggleMyAnonDefault(CancellationToken ct)
    {
        var userId = GetUserIdFromToken();
        if (userId is null) return Unauthorized();

        await toggleUserAnonDefaultUseCase.ExecuteAsync(userId.Value, ct);
        return NoContent();
    }

    [HttpPatch("me/anon-image/{anonImageId:guid}")]
    public async Task<IActionResult> AssignMyAnonImage(Guid anonImageId, CancellationToken ct)
    {
        var userId = GetUserIdFromToken();
        if (userId is null) return Unauthorized();

        await assignAnonImageToUserUseCase.ExecuteAsync(userId.Value, anonImageId, ct);
        return NoContent();
    }

    [HttpPut("{id}")]
    [Authorize(Policy = "Permission:users.update")]
    public async Task<IActionResult> UpdateUser(
        Guid id,
        [FromBody] UpdateUserRequestDto req,
        CancellationToken ct)
    {
        var result = await updateUserUseCase.ExecuteAsync(id, req, ct);
        return Ok(result);
    }

    [HttpDelete("me")]
    public async Task<IActionResult> DeleteMe(CancellationToken ct)
    {
        var userId = GetUserIdFromToken();
        if (userId is null) return Unauthorized();

        await deleteUserUseCase.ExecuteAsync(userId.Value, ct);
        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = "Permission:users.delete")]
    public async Task<IActionResult> DeleteUser(Guid id, CancellationToken ct)
    {
        await deleteUserUseCase.ExecuteAsync(id, ct);
        return NoContent();
    }

    [HttpDelete("{id:guid}/permanent")]
    [Authorize(Policy = "Permission:users.delete-permanent")]
    public async Task<IActionResult> DeleteUserPermanent(Guid id, CancellationToken ct)
    {
        try
        {
            await deleteUserUseCasePermanent.ExecuteAsync(id, ct);
            return NoContent();
        }
        catch (Exception ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpGet("{userId:guid}/roles")]
    [Authorize(Policy = "Permission:users.read-roles")]
    public async Task<IActionResult> GetUserRoles(Guid userId, CancellationToken ct)
    {
        try
        {
            return Ok(await getUserRolesUseCase.ExecuteAsync(userId, ct));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpPost("{userId:guid}/roles/{roleId:guid}")]
    [Authorize(Policy = "Permission:users.assign-role")]
    public async Task<IActionResult> AssignRole(Guid userId, Guid roleId, CancellationToken ct)
    {
        try
        {
            await assignRoleToUserUseCase.ExecuteAsync(userId, roleId, ct);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpDelete("{userId:guid}/roles/{roleId:guid}")]
    [Authorize(Policy = "Permission:users.remove-role")]
    public async Task<IActionResult> RemoveRole(Guid userId, Guid roleId, CancellationToken ct)
    {
        try
        {
            await removeRoleFromUserUseCase.ExecuteAsync(userId, roleId, ct);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }
}
