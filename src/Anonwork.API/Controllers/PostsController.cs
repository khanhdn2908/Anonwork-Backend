using Anonwork.API.DTOs;
using Anonwork.Application.Common.Exceptions;
using Anonwork.Application.Features.PostRatings;
using Anonwork.Application.Features.PostRatings.DTOs.Requests;
using Anonwork.Application.Features.Posts;
using Anonwork.Application.Features.Posts.DTOs.Request;
using Anonwork.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Anonwork.API.Controllers;

[ApiController]
[Route("api/v1/posts")]
[Authorize]
public class PostsController(
    CreatePostUseCase createPostUseCase,
    GetPostByIdUseCase getPostByIdUseCase,
    GetPostsUseCase getPostsUseCase,
    GetTopPostsByTimeUseCase getTopPostsByTimeUseCase,
    UpdatePostUseCase updatePostUseCase,
    DeletePostUseCase deletePostUseCase,
    DeletePostUseCasePermanent deletePostUseCasePermanent,
    TogglePostVoteUseCase togglePostVoteUseCase,
    RatePostUseCase ratePostUseCase,
    GetPostRatingSummaryUseCase getPostRatingSummaryUseCase,
    DeletePostRatingUseCase deletePostRatingUseCase,
    IAuthorizationService authorizationService) : BaseApiController
{

    [HttpPost]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Create([FromForm] CreatePostRequestDto req, CancellationToken ct)
    {
        var userId = GetUserIdFromToken();
        if (userId is null)
            return Unauthorized(new { message = "User not authenticated" });

        // ── Create post ─────────────────────────────
        var request = new CreatePostRequest(
            AuthorId: userId.Value,
            Title: req.Title,
            Content: req.Content,
            SubjectId: req.SubjectId,
            Tags: req.Tags,
            Images: req.Images,
            File: req.File,
            IsAnonymous: req.IsAnonymous
        );

        var result = await createPostUseCase.ExecuteAsync(request, ct);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }


    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        try
        {
            var userId = GetUserIdFromToken();
            var authResult = await authorizationService.AuthorizeAsync(User, "Permission:posts.read:all");

            var result = await getPostByIdUseCase.ExecuteAsync(id, authResult.Succeeded, userId, ct);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }


    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? search = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken ct = default)
    {
        var userId = GetUserIdFromToken();
        var authResult = await authorizationService.AuthorizeAsync(User, "Permission:posts.read:all");

        var result = await getPostsUseCase.ExecuteAsync(authResult.Succeeded, page, pageSize, search, userId, ct);
        return Ok(result);
    }

    [HttpGet("top")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetTopByTime(
        [FromQuery] string range = "24h",
        [FromQuery] string sort = "hot",
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken ct = default)
    {
        var userId = GetUserIdFromToken();
        var authResult = await authorizationService.AuthorizeAsync(User, "Permission:posts.read:all");

        var result = await getTopPostsByTimeUseCase.ExecuteAsync(authResult.Succeeded, range, sort, page, pageSize, userId, ct);
        return Ok(result);
    }


    [HttpPut("{id:guid}")]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromForm] UpdatePostRequestDto req, CancellationToken ct)
    {
        var userId = GetUserIdFromToken();
        if (userId is null)
            return Unauthorized(new { message = "User not authenticated" });

        // ── Update post ─────────────────────────────
        var request = new UpdatePostRequest(
            PostId: id,
            AuthorId: userId.Value,
            Title: req.Title,
            Content: req.Content,
            Tags: req.Tags,
            Images: req.NewImages,
            Files: req.NewFiles,
            RemoveMediaId: req.RemoveFileId
        );

        try
        {
            var result = await updatePostUseCase.ExecuteAsync(request, ct);
            return Ok(result);
        }
        catch (UnauthorizedException ex)
        {
            return Forbid();
        }
        catch (Exception ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }


    [HttpDelete("{id:guid}")]
    [Authorize(Policy = "Permission:posts.delete")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var userId = GetUserIdFromToken();
        if (userId is null)
            return Unauthorized(new { message = "User not authenticated" });

        try
        {
            await deletePostUseCase.ExecuteAsync(id, userId.Value, ct);
            return NoContent();
        }
        catch (UnauthorizedException ex)
        {
            return Forbid();
        }
        catch (Exception ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpDelete("{id:guid}/permanent")]
    [Authorize(Policy = "Permission:posts.delete-permanent")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeletePermanent(Guid id, CancellationToken ct)
    {
        try
        {
            await deletePostUseCasePermanent.ExecuteAsync(id, ct);
            return NoContent();
        }
        catch (Exception ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }


    [HttpPost("{id:guid}/upvote")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ToggleUpvote(Guid id, CancellationToken ct)
    {
        var userId = GetUserIdFromToken();
        if (userId is null)
            return Unauthorized(new { message = "User not authenticated" });

        try
        {
            var result = await togglePostVoteUseCase.ExecuteAsync(userId.Value, id, ct);
            return Ok(result);
        }
        catch (Exception ex) when (ex.Message.Contains("not found", StringComparison.OrdinalIgnoreCase))
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpPost("{id:guid}/rate")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RatePost(Guid id, [FromBody] RatePostRequestDto req, CancellationToken ct)
    {
        var userId = GetUserIdFromToken();
        if (userId is null)
            return Unauthorized(new { message = "User not authenticated" });

        try
        {
            var result = await ratePostUseCase.ExecuteAsync(userId.Value, id, req, ct);
            return Ok(result);
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("{id:guid}/ratings")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetRatings(Guid id, CancellationToken ct)
    {
        var userId = GetUserIdFromToken();
        try
        {
            var result = await getPostRatingSummaryUseCase.ExecuteAsync(id, userId, ct);
            return Ok(result);
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpDelete("{id:guid}/rate")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteRating(Guid id, CancellationToken ct)
    {
        var userId = GetUserIdFromToken();
        if (userId is null)
            return Unauthorized(new { message = "User not authenticated" });

        try
        {
            var result = await deletePostRatingUseCase.ExecuteAsync(userId.Value, id, ct);
            return Ok(result);
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }
}

