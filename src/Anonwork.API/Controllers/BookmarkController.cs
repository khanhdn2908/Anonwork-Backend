using Anonwork.Application.Features.Bookmarks;
using Anonwork.Application.Features.Bookmarks.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Anonwork.API.Controllers;

[ApiController]
[Route("api/v1/bookmarks")]
[Authorize]
public class BookmarkController(
    CreateBookmarkUseCase createBookmarkUseCase,
    DeleteBookmarkUseCase deleteBookmarkUseCase,
    GetBookmarksUseCase getBookmarksUseCase,
    IsBookmarkedUseCase isBookmarkedUseCase) : BaseApiController
{
    /// <summary>
    /// Create a bookmark for a post
    /// </summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Create([FromBody] CreateBookmarkRequest req, CancellationToken ct)
    {
        var userId = GetUserIdFromToken();
        if (userId is null)
            return Unauthorized(new { message = "User not authenticated" });

        try
        {
            var result = await createBookmarkUseCase.ExecuteAsync(userId.Value, req, ct);
            return CreatedAtAction(nameof(GetAll), new { }, result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Delete bookmark for a post
    /// </summary>
    [HttpDelete("{postId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid postId, CancellationToken ct)
    {
        var userId = GetUserIdFromToken();
        if (userId is null)
            return Unauthorized(new { message = "User not authenticated" });

        try
        {
            await deleteBookmarkUseCase.ExecuteAsync(userId.Value, postId, ct);
            return NoContent();
        }
        catch (Exception ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Get current user's bookmarks
    /// </summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10, CancellationToken ct = default)
    {
        var userId = GetUserIdFromToken();
        if (userId is null)
            return Unauthorized(new { message = "User not authenticated" });

        var result = await getBookmarksUseCase.ExecuteAsync(userId.Value, page, pageSize, ct);
        return Ok(result);
    }

    /// <summary>
    /// Check bookmark status for a post
    /// </summary>
    [HttpGet("{postId:guid}/exists")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Exists(Guid postId, CancellationToken ct)
    {
        var userId = GetUserIdFromToken();
        if (userId is null)
            return Unauthorized(new { message = "User not authenticated" });

        var result = await isBookmarkedUseCase.ExecuteAsync(userId.Value, postId, ct);
        return Ok(new { isBookmarked = result });
    }
}