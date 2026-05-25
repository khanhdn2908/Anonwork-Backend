using Anonwork.Application.Features.Posts;
using Anonwork.Application.Features.Posts.DTOs;
using Anonwork.Application.Features.Subjects;
using Anonwork.Application.Features.Subjects.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Anonwork.API.Controllers;

[ApiController]
[Route("api/v1/subjects")]
[Authorize]
public class SubjectsController(
    GetSubjectsUseCase getSubjectsUseCase,
    GetSubjectByIdUseCase getSubjectByIdUseCase,
    CreateSubjectUseCase createSubjectUseCase,
    UpdateSubjectUseCase updateSubjectUseCase,
    DeleteSubjectUseCase deleteSubjectUseCase,
    GetPostsBySubjectUseCase getPostsBySubjectUseCase) : ControllerBase
{
    /// <summary>
    /// Get all subjects with search and pagination
    /// </summary>
    /// <remarks>
    /// Retrieves a paginated list of subjects, sorted by post count (descending) and creation date.
    /// Supports search by name or slug.
    /// 
    /// Sample request:
    /// 
    ///     GET /api/v1/subjects?search=csharp&page=1&pageSize=10
    /// </remarks>
    /// <param name="search">Search query (optional, searches in name and slug)</param>
    /// <param name="page">Page number (default: 1)</param>
    /// <param name="pageSize">Items per page (default: 10, max: 100)</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Paginated list of subjects</returns>
    /// <response code="200">Subjects retrieved successfully</response>
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? search = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken ct = default)
    {
        var result = await getSubjectsUseCase.ExecuteAsync(search, page, pageSize, ct);
        return Ok(result);
    }

    /// <summary>
    /// Get a subject by ID
    /// </summary>
    /// <remarks>
    /// Retrieves a specific subject by its ID.
    /// 
    /// Sample request:
    /// 
    ///     GET /api/v1/subjects/550e8400-e29b-41d4-a716-446655440000
    /// </remarks>
    /// <param name="id">Subject ID</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Subject details</returns>
    /// <response code="200">Subject found</response>
    /// <response code="404">Subject not found</response>
    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct = default)
    {
        try
        {
            var result = await getSubjectByIdUseCase.ExecuteAsync(id, ct);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Create a new subject
    /// </summary>
    /// <remarks>
    /// Creates a new subject. Requires authentication and admin role.
    /// 
    /// Sample request:
    /// 
    ///     POST /api/v1/subjects
    ///     {
    ///       "name": "C# Programming",
    ///       "slug": "csharp-programming",
    ///       "iconEmoji": "🔷"
    ///     }
    /// </remarks>
    /// <param name="request">Subject creation request</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Created subject with 201 status</returns>
    /// <response code="201">Subject created successfully</response>
    /// <response code="400">Invalid request data</response>
    /// <response code="401">Unauthorized</response>
    [Authorize(Roles = "admin")]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Create(
        [FromBody] CreateSubjectRequestDto request,
        CancellationToken ct = default)
    {
        try
        {
            var result = await createSubjectUseCase.ExecuteAsync(request, ct);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Update a subject
    /// </summary>
    /// <remarks>
    /// Updates an existing subject. Requires authentication and admin role.
    /// 
    /// Sample request:
    /// 
    ///     PUT /api/v1/subjects/550e8400-e29b-41d4-a716-446655440000
    ///     {
    ///       "name": "C# Advanced",
    ///       "slug": "csharp-advanced",
    ///       "iconEmoji": "🔷"
    ///     }
    /// </remarks>
    /// <param name="id">Subject ID</param>
    /// <param name="request">Subject update request</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Updated subject</returns>
    /// <response code="200">Subject updated successfully</response>
    /// <response code="400">Invalid request data</response>
    /// <response code="401">Unauthorized</response>
    /// <response code="404">Subject not found</response>
    [Authorize(Roles = "admin")]
    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateSubjectRequestDto request,
        CancellationToken ct = default)
    {
        try
        {
            var result = await updateSubjectUseCase.ExecuteAsync(id, request, ct);
            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Delete a subject
    /// </summary>
    /// <remarks>
    /// Deletes a subject. Requires authentication and admin role.
    /// 
    /// Sample request:
    /// 
    ///     DELETE /api/v1/subjects/550e8400-e29b-41d4-a716-446655440000
    /// </remarks>
    /// <param name="id">Subject ID</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>No content</returns>
    /// <response code="204">Subject deleted successfully</response>
    /// <response code="401">Unauthorized</response>
    /// <response code="404">Subject not found</response>
    [Authorize(Roles = "admin")]
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct = default)
    {
        try
        {
            await deleteSubjectUseCase.ExecuteAsync(id, ct);
            return NoContent();
        }
        catch (Exception ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Get posts by subject with pagination
    /// </summary>
    /// <remarks>
    /// Retrieves a paginated list of posts for a specific subject, sorted by creation date (newest first).
    /// 
    /// Sample request:
    /// 
    ///     GET /api/v1/subjects/550e8400-e29b-41d4-a716-446655440000/posts?page=1&pageSize=10
    /// </remarks>
    /// <param name="subjectId">Subject ID</param>
    /// <param name="page">Page number (default: 1)</param>
    /// <param name="pageSize">Items per page (default: 10, max: 100)</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Paginated list of posts for the subject</returns>
    /// <response code="200">Posts retrieved successfully</response>
    /// <response code="404">Subject not found</response>
    [HttpGet("{subjectId:guid}/posts")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPosts(
        Guid subjectId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken ct = default)
    {
        try
        {
            var result = await getPostsBySubjectUseCase.ExecuteAsync(subjectId, page, pageSize, ct);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }
}
