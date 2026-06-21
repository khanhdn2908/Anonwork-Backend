using Anonwork.Application.Features.Follows;
using Anonwork.Application.Features.Follows.DTOs.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Anonwork.API.Controllers;

[ApiController]
[Route("api/v1/follows")]
[Authorize]
public class FollowController(
    FollowUserUseCase followUserUseCase,
    UnfollowUserUseCase unfollowUserUseCase,
    GetFollowByIdUseCase getFollowByIdUseCase,
    GetFollowersUseCase getFollowersUseCase,
    GetFollowingUseCase getFollowingUseCase,
    GetFollowStatsUseCase getFollowStatsUseCase,
    IsFollowingUseCase isFollowingUseCase,
    IAuthorizationService authorizationService) : BaseApiController
{

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Follow([FromBody] FollowUserRequest request, CancellationToken ct)
    {
        var userId = GetUserIdFromToken();
        if (userId is null)
            return Unauthorized(new { message = "User not authenticated" });

        try
        {
            var result = await followUserUseCase.ExecuteAsync(userId.Value, request, ct);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("{followingId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Unfollow(Guid followingId, CancellationToken ct)
    {
        var userId = GetUserIdFromToken();
        if (userId is null)
            return Unauthorized(new { message = "User not authenticated" });

        try
        {
            await unfollowUserUseCase.ExecuteAsync(userId.Value, followingId, ct);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        try
        {
            var result = await getFollowByIdUseCase.ExecuteAsync(id, ct);
            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("followers/{userId:guid}")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetFollowers(
        Guid userId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken ct = default)
    {
        var authResult = await authorizationService.AuthorizeAsync(User, "Permission:users.read:all");

        try
        {
            var result = await getFollowersUseCase.ExecuteAsync(userId, authResult.Succeeded, page, pageSize, ct);
            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }


    [HttpGet("following/{userId:guid}")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetFollowing(
        Guid userId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken ct = default)
    {
        var authResult = await authorizationService.AuthorizeAsync(User, "Permission:users.read:all");

        try
        {
            var result = await getFollowingUseCase.ExecuteAsync(userId, authResult.Succeeded, page, pageSize, ct);
            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("stats/{userId:guid}")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetStats(Guid userId, CancellationToken ct)
    {
        try
        {
            var currentUserId = GetUserIdFromToken();
            var result = await getFollowStatsUseCase.ExecuteAsync(userId, currentUserId, ct);
            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("is-following/{followingId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> IsFollowing(Guid followingId, CancellationToken ct)
    {
        var userId = GetUserIdFromToken();
        if (userId is null)
            return Unauthorized(new { message = "User not authenticated" });

        try
        {
            var result = await isFollowingUseCase.ExecuteAsync(userId.Value, followingId, ct);
            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
