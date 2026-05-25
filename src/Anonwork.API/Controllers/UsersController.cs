using Anonwork.Application.Features.Users;
using Anonwork.Application.Features.Users.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Anonwork.API.Controllers;

[ApiController]
[Route("api/v1/users")]
[Authorize]
public class UsersController(
    GetMeUseCase getMeUseCase,
    UpdateUserUseCase updateUserUseCase,
    DeleteUserUseCase deleteUserUseCase,
    GetAllUsersUseCase getAllUsersUseCase) : BaseApiController
{
    /// <summary>
    /// Get current user profile
    /// </summary>
    [HttpGet("me")]
    public async Task<IActionResult> GetMe(CancellationToken ct)
    {
        var userId = GetUserIdFromToken();
        if (userId is null) return Unauthorized();

        var result = await getMeUseCase.ExecuteAsync(userId.Value, ct);
        return Ok(result);
    }

    /// <summary>
    /// Get user by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetUserById(Guid id, CancellationToken ct)
    {
        var result = await getMeUseCase.ExecuteAsync(id, ct);
        return Ok(result);
    }

    /// <summary>
    /// Get all users (paginated)
    /// </summary>
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAllUsers(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken ct = default)
    {
        var result = await getAllUsersUseCase.ExecuteAsync(page, pageSize, ct);
        return Ok(result);
    }

    /// <summary>
    /// Update current user profile
    /// </summary>
    [HttpPut("me")]
    public async Task<IActionResult> UpdateMe(
        [FromBody] UpdateUserRequestDto req,
        CancellationToken ct)
    {
        var userId = GetUserIdFromToken();
        if (userId is null) return Unauthorized();

        var result = await updateUserUseCase.ExecuteAsync(userId.Value, req, ct);
        return Ok(result);
    }

    /// <summary>
    /// Update user by ID (admin only)
    /// </summary>
    [Authorize(Roles = "admin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUser(
        Guid id,
        [FromBody] UpdateUserRequestDto req,
        CancellationToken ct)
    {

        var result = await updateUserUseCase.ExecuteAsync(id, req, ct);
        return Ok(result);
    }

    /// <summary>
    /// Delete current user account
    /// </summary>
    [HttpDelete("me")]
    public async Task<IActionResult> DeleteMe(CancellationToken ct)
    {
        var userId = GetUserIdFromToken();
        if (userId is null) return Unauthorized();

        await deleteUserUseCase.ExecuteAsync(userId.Value, ct);
        return NoContent();
    }

    /// <summary>
    /// Delete user by ID
    /// </summary>
    [Authorize(Roles = "admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(Guid id, CancellationToken ct)
    {
        var userId = GetUserIdFromToken();
        if (userId is null) return Unauthorized();

        await deleteUserUseCase.ExecuteAsync(id, ct);
        return NoContent();
    }

}
