using Anonwork.Application.Features.Comments;
using Anonwork.Application.Features.Comments.DTOs.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Anonwork.API.Controllers;

[ApiController]
[Route("api/v1/comments")]
[Authorize]
public class CommentController(
    CreateCommentUseCase createCommentUseCase,
    GetCommentsByPostUseCase getCommentsByPostUseCase,
    UpdateCommentUseCase updateCommentUseCase,
    DeleteCommentUseCase deleteCommentUseCase,
    DeleteCommentUseCasePermanent deleteCommentUseCasePermanent,
    ToggleCommentVoteUseCase toggleCommentVoteUseCase,
    IAuthorizationService authorizationService) : BaseApiController
{

    /// <summary>
    /// Get comments by post
    /// </summary>
    [HttpGet("post/{postId:guid}")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetAllByPost(Guid postId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10, CancellationToken ct = default)
    {
        var authResult = await authorizationService.AuthorizeAsync(User, "Permission:comments.read:all");

        var result = await getCommentsByPostUseCase.ExecuteAsync(postId, authResult.Succeeded, page, pageSize, ct);
        return Ok(result);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Create([FromBody] CreateCommentRequest req, CancellationToken ct)
    {
        var userId = GetUserIdFromToken();
        if (userId is null)
            return Unauthorized(new { message = "User not authenticated" });

        try
        {
            var result = await createCommentUseCase.ExecuteAsync(userId.Value, req, ct);
            return CreatedAtAction(nameof(GetAllByPost), new { postId = req.PostId }, result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Toggle upvote for a comment
    /// </summary>
    [HttpPost("{commentId:guid}/upvote")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ToggleUpvote(Guid commentId, CancellationToken ct)
    {
        var userId = GetUserIdFromToken();
        if (userId is null)
            return Unauthorized(new { message = "User not authenticated" });

        try
        {
            var result = await toggleCommentVoteUseCase.ExecuteAsync(userId.Value, commentId, ct);
            return Ok(result);
        }
        catch (Exception ex) when (ex.Message.Contains("not found", StringComparison.OrdinalIgnoreCase))
        {
            return NotFound(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Update a comment
    /// </summary>
    [HttpPut("{commentId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid commentId, [FromBody] string content, CancellationToken ct)
    {
        var userId = GetUserIdFromToken();
        if (userId is null)
            return Unauthorized(new { message = "User not authenticated" });

        try
        {
            var result = await updateCommentUseCase.ExecuteAsync(userId.Value, commentId, content, ct);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Delete a comment
    /// </summary>
    [HttpDelete("{commentId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid commentId, CancellationToken ct)
    {
        var userId = GetUserIdFromToken();
        if (userId is null)
            return Unauthorized(new { message = "User not authenticated" });

        try
        {
            await deleteCommentUseCase.ExecuteAsync(userId.Value, commentId, ct);
            return NoContent();
        }
        catch (Exception ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpDelete("{commentId:guid}/permanent")]
    [Authorize(Policy = "Permission:comments.delete-permanent")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeletePermanent(Guid commentId, CancellationToken ct)
    {
        var userId = GetUserIdFromToken();
        if (userId is null)
            return Unauthorized(new { message = "User not authenticated" });

        try
        {
            await deleteCommentUseCasePermanent.ExecuteAsync(userId.Value, commentId, ct);
            return NoContent();
        }
        catch (Exception ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }
}
