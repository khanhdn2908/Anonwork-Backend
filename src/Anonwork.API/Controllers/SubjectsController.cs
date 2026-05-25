using Anonwork.Application.Features.Posts;
using Anonwork.Application.Features.Posts.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Anonwork.API.Controllers;

[ApiController]
[Route("api/v1/subjects")]
[Authorize]
public class SubjectsController(GetPostsBySubjectUseCase getPostsBySubjectUseCase) : ControllerBase
{
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
