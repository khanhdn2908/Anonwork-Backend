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
    UpdateUserUseCase updateUserUseCase,
    ToggleUserAnonDefaultUseCase toggleUserAnonDefaultUseCase,
    AssignAnonImageToUserUseCase assignAnonImageToUserUseCase,
    DeleteUserUseCase deleteUserUseCase,
    GetAllUsersUseCase getAllUsersUseCase,
    AssignRoleToUserUseCase assignRoleToUserUseCase,
    RemoveRoleFromUserUseCase removeRoleFromUserUseCase,
    GetUserRolesUseCase getUserRolesUseCase) : BaseApiController
{
    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> GetMe(CancellationToken ct)
    {
        var userId = GetUserIdFromToken();
        if (userId is null) return Unauthorized();

        var result = await getMeUseCase.ExecuteAsync(userId.Value, ct);
        return Ok(result);
    }

    [HttpGet("{id}")]
    [Authorize(Policy = "Permission:users.read")]
    public async Task<IActionResult> GetUserById(Guid id, CancellationToken ct)
    {
        var result = await getMeUseCase.ExecuteAsync(id, ct);
        return Ok(result);
    }

    [HttpGet]
    [Authorize(Policy = "Permission:users.read")]
    public async Task<IActionResult> GetAllUsers(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken ct = default)
    {
        var result = await getAllUsersUseCase.ExecuteAsync(page, pageSize, ct);
        return Ok(result);
    }

    [HttpPut("me")]
    [Consumes("multipart/form-data")]
    [Authorize]
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
    [Authorize]
    public async Task<IActionResult> ToggleMyAnonDefault(CancellationToken ct)
    {
        var userId = GetUserIdFromToken();
        if (userId is null) return Unauthorized();

        await toggleUserAnonDefaultUseCase.ExecuteAsync(userId.Value, ct);
        return NoContent();
    }

    [HttpPatch("me/anon-image/{anonImageId:guid}")]
    [Authorize]
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
    [Authorize]
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
