using Anonwork.API.DTOs;
using Anonwork.Application.Common.Exceptions;
using Anonwork.Application.Features.Posts;
using Anonwork.Application.Features.Posts.DTOs;
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
    SearchPostsUseCase searchPostsUseCase,
    UpdatePostUseCase updatePostUseCase,
    DeletePostUseCase deletePostUseCase,
    ICloudinaryService cloudinaryService) : BaseApiController
{
    /// <summary>
    /// Create a new post with optional images
    /// </summary>
    /// <remarks>
    /// Requires authentication. User must be logged in to create a post.
    /// 
    /// Sample request:
    /// 
    ///     POST /api/v1/posts
    ///     {
    ///       "title": "Cách học C# hiệu quả",
    ///       "content": "Bài viết chi tiết về cách học C#...",
    ///       "subjectId": "550e8400-e29b-41d4-a716-446655440000",
    ///       "tags": ["csharp", "learning"],
    ///       "isAnonymous": false,
    ///       "images": [file1.jpg, file2.png]
    ///     }
    /// </remarks>
    /// <param name="req">Post creation request</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Created post with 201 status</returns>
    /// <response code="201">Post created successfully</response>
    /// <response code="400">Invalid request data</response>
    /// <response code="401">Unauthorized</response>
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

    /// <summary>
    /// Get a post by ID
    /// </summary>
    /// <remarks>
    /// Retrieves a specific post by its ID. View count is automatically incremented.
    /// 
    /// Sample request:
    /// 
    ///     GET /api/v1/posts/550e8400-e29b-41d4-a716-446655440001
    /// </remarks>
    /// <param name="id">Post ID</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Post details</returns>
    /// <response code="200">Post found</response>
    /// <response code="404">Post not found</response>
    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        try
        {
            var result = await getPostByIdUseCase.ExecuteAsync(id, ct);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Get all posts with pagination
    /// </summary>
    /// <remarks>
    /// Retrieves a paginated list of all active posts, sorted by creation date (newest first).
    /// 
    /// Sample request:
    /// 
    ///     GET /api/v1/posts?page=1&pageSize=10
    /// </remarks>
    /// <param name="page">Page number (default: 1)</param>
    /// <param name="pageSize">Items per page (default: 10, max: 100)</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Paginated list of posts</returns>
    /// <response code="200">Posts retrieved successfully</response>
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10, CancellationToken ct = default)
    {
        var result = await getPostsUseCase.ExecuteAsync(page, pageSize, ct);
        return Ok(result);
    }

    /// <summary>
    /// Search posts by query using full-text search
    /// </summary>
    /// <remarks>
    /// Searches posts by title and content using PostgreSQL full-text search.
    /// Results are ranked by relevance and sorted by creation date.
    /// 
    /// Sample request:
    /// 
    ///     GET /api/v1/posts/search?q=C%23&page=1&pageSize=10
    /// </remarks>
    /// <param name="q">Search query (minimum 2 characters)</param>
    /// <param name="page">Page number (default: 1)</param>
    /// <param name="pageSize">Items per page (default: 10, max: 100)</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Paginated search results</returns>
    /// <response code="200">Search results retrieved successfully</response>
    /// <response code="400">Invalid search query</response>
    [HttpGet("search")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Search(
        [FromQuery] string q,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken ct = default)
    {
        try
        {
            var result = await searchPostsUseCase.ExecuteAsync(q, page, pageSize, ct);
            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Update a post
    /// </summary>
    /// <remarks>
    /// Updates an existing post. Only the post author can update their own posts.
    /// 
    /// Sample request:
    /// 
    ///     PUT /api/v1/posts/550e8400-e29b-41d4-a716-446655440001
    ///     {
    ///       "title": "Tiêu đề mới",
    ///       "content": "Nội dung mới...",
    ///       "tags": ["tag1", "tag2"],
    ///       "newImages": [file.jpg],
    ///       "removeImageUrls": ["https://..."]
    ///     }
    /// </remarks>
    /// <param name="id">Post ID</param>
    /// <param name="req">Update request</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Updated post</returns>
    /// <response code="200">Post updated successfully</response>
    /// <response code="400">Invalid request data</response>
    /// <response code="401">Unauthorized</response>
    /// <response code="403">Forbidden - not the post author</response>
    /// <response code="404">Post not found</response>
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

    /// <summary>
    /// Delete a post
    /// </summary>
    /// <remarks>
    /// Deletes a post (soft delete). Only the post author can delete their own posts.
    /// 
    /// Sample request:
    /// 
    ///     DELETE /api/v1/posts/550e8400-e29b-41d4-a716-446655440001
    /// </remarks>
    /// <param name="id">Post ID</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>No content</returns>
    /// <response code="204">Post deleted successfully</response>
    /// <response code="401">Unauthorized</response>
    /// <response code="403">Forbidden - not the post author</response>
    /// <response code="404">Post not found</response>
    [HttpDelete("{id:guid}")]
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

    // ── Helpers ─────────────────────────────────────────
}

