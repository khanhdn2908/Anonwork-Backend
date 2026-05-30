using Anonwork.Application.Features.Follows;
using Anonwork.Application.Features.Follows.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Anonwork.API.Controllers;

[ApiController]
[Route("api/v1/follows")]
public class FollowController(
    FollowUserUseCase followUserUseCase,
    UnfollowUserUseCase unfollowUserUseCase,
    GetFollowByIdUseCase getFollowByIdUseCase,
    GetFollowersUseCase getFollowersUseCase,
    GetFollowingUseCase getFollowingUseCase,
    GetFollowStatsUseCase getFollowStatsUseCase,
    IsFollowingUseCase isFollowingUseCase) : BaseApiController
{

    /// </remarks>
    /// <param name="request">Follow request containing the user ID to follow</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Created follow relationship with 201 status</returns>
    /// <response code="201">Follow relationship created successfully</response>
    /// <response code="400">Invalid request or already following</response>
    /// <response code="401">Unauthorized</response>
    /// <response code="404">User to follow not found</response>
    [HttpPost]
    [Authorize]
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

    /// <summary>
    /// Unfollow a user
    /// </summary>
    /// <remarks>
    /// Requires authentication. Removes the follow relationship between the current user and the target user.
    /// 
    /// Sample request:
    /// 
    ///     DELETE /api/v1/follows/550e8400-e29b-41d4-a716-446655440000
    /// </remarks>
    /// <param name="followingId">ID of the user to unfollow</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>No content on success</returns>
    /// <response code="204">Unfollow successful</response>
    /// <response code="401">Unauthorized</response>
    /// <response code="404">Follow relationship not found</response>
    [HttpDelete("{followingId:guid}")]
    [Authorize]
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

    /// <summary>
    /// Get a follow relationship by ID
    /// </summary>
    /// <remarks>
    /// Retrieves details of a specific follow relationship.
    /// 
    /// Sample request:
    /// 
    ///     GET /api/v1/follows/550e8400-e29b-41d4-a716-446655440001
    /// </remarks>
    /// <param name="id">Follow relationship ID</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Follow relationship details</returns>
    /// <response code="200">Follow relationship found</response>
    /// <response code="404">Follow relationship not found</response>
    [HttpGet("{id:guid}")]
    [AllowAnonymous]
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

    /// <summary>
    /// Get followers of a user with pagination
    /// </summary>
    /// <remarks>
    /// Retrieves a paginated list of all followers of a user.
    /// 
    /// Sample request:
    /// 
    ///     GET /api/v1/follows/followers/550e8400-e29b-41d4-a716-446655440000?page=1&pageSize=10
    /// </remarks>
    /// <param name="userId">User ID to get followers for</param>
    /// <param name="page">Page number (default: 1)</param>
    /// <param name="pageSize">Items per page (default: 10, max: 100)</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Paginated list of followers</returns>
    /// <response code="200">Followers retrieved successfully</response>
    /// <response code="400">Invalid pagination parameters</response>
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
        try
        {
            var result = await getFollowersUseCase.ExecuteAsync(userId, page, pageSize, ct);
            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Get users that a user is following with pagination
    /// </summary>
    /// <remarks>
    /// Retrieves a paginated list of all users that a user is following.
    /// 
    /// Sample request:
    /// 
    ///     GET /api/v1/follows/following/550e8400-e29b-41d4-a716-446655440000?page=1&pageSize=10
    /// </remarks>
    /// <param name="userId">User ID to get following list for</param>
    /// <param name="page">Page number (default: 1)</param>
    /// <param name="pageSize">Items per page (default: 10, max: 100)</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Paginated list of users being followed</returns>
    /// <response code="200">Following list retrieved successfully</response>
    /// <response code="400">Invalid pagination parameters</response>
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
        try
        {
            var result = await getFollowingUseCase.ExecuteAsync(userId, page, pageSize, ct);
            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Get follow statistics for a user
    /// </summary>
    /// <remarks>
    /// Retrieves follow statistics including follower count, following count, and whether the current user is following this user.
    /// 
    /// Sample request:
    /// 
    ///     GET /api/v1/follows/stats/550e8400-e29b-41d4-a716-446655440000
    /// </remarks>
    /// <param name="userId">User ID to get statistics for</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Follow statistics</returns>
    /// <response code="200">Statistics retrieved successfully</response>
    /// <response code="400">Invalid user ID</response>
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

    /// <summary>
    /// Check if the current user is following a specific user
    /// </summary>
    /// <remarks>
    /// Checks whether the current user is following the specified user.
    /// 
    /// Sample request:
    /// 
    ///     GET /api/v1/follows/is-following/550e8400-e29b-41d4-a716-446655440000
    /// </remarks>
    /// <param name="followingId">User ID to check if following</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Boolean indicating if following</returns>
    /// <response code="200">Check completed successfully</response>
    /// <response code="401">Unauthorized</response>
    /// <response code="400">Invalid user ID</response>
    [HttpGet("is-following/{followingId:guid}")]
    [Authorize]
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
