using Anonwork.API.DTOs;
using Anonwork.Application.Common.Exceptions;
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
    GetPostsBySubjectUseCase getPostsBySubjectUseCase,
    UpdatePostUseCase updatePostUseCase,
    DeletePostUseCase deletePostUseCase,
    DeletePostUseCasePermanent deletePostUseCasePermanent,
    TogglePostVoteUseCase togglePostVoteUseCase,
    ICloudinaryService cloudinaryService) : BaseApiController
{

    [HttpPost]
    [Authorize(Policy = "Permission:posts.create")]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Create([FromForm] CreatePostRequestDto req, CancellationToken ct)
    {
        var userId = GetUserIdFromToken();
        if (userId is null)
            return Unauthorized(new { message = "User not authenticated" });

        // ── Upload images if provided ───────────────
        List<string>? imageUrls = null;
        if (req.Images is not null && req.Images.Count > 0)
        {
            try
            {
                imageUrls = await cloudinaryService.UploadImagesAsync(req.Images, "posts", ct);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Failed to upload images", error = ex.Message });
            }
        }

        // ── Create post ─────────────────────────────
        var request = new CreatePostRequest(
            AuthorId: userId.Value,
            Title: req.Title,
            Content: req.Content,
            SubjectId: req.SubjectId,
            Tags: req.Tags,
            ImageUrls: imageUrls,
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
            var result = await getPostByIdUseCase.ExecuteAsync(id, userId, ct);
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
        var permissions = GetPermissionsFromToken();
        var result = await getPostsUseCase.ExecuteAsync(page, pageSize, search, userId, permissions, ct);
        return Ok(result);
    }


    [HttpPut("{id:guid}")]
    [Authorize(Policy = "Permission:posts.update")]
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

        // ── Upload new images if provided ───────────
        List<string>? newImageUrls = null;
        if (req.NewImages is not null && req.NewImages.Count > 0)
        {
            try
            {
                newImageUrls = await cloudinaryService.UploadImagesAsync(req.NewImages, "posts", ct);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Failed to upload images", error = ex.Message });
            }
        }

        // ── Update post ─────────────────────────────
        var request = new UpdatePostRequest(
            PostId: id,
            AuthorId: userId.Value,
            Title: req.Title,
            Content: req.Content,
            Tags: req.Tags,
            NewImageUrls: newImageUrls,
            RemoveImageUrls: req.RemoveImageUrls
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
    [Authorize(Policy = "Permission:posts.vote")]
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

    // ── Helpers ─────────────────────────────────────────
    private IReadOnlyCollection<string> GetPermissionsFromToken()
    {
        return User.Claims
            .Where(c => c.Type == "permission")
            .Select(c => c.Value)
            .ToArray();
    }
}

