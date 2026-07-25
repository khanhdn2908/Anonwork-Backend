using Anonwork.Application.Features.Search;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Anonwork.API.Controllers;

[ApiController]
[Route("api/v1/search")]
[Authorize]
public class SearchController(
    SearchAllUseCase searchAllUseCase,
    SearchPostsUseCase searchPostsUseCase,
    SearchUsersUseCase searchUsersUseCase,
    IAuthorizationService authorizationService) : BaseApiController
{
    /// <summary>
    /// Global search endpoint for both posts and users.
    /// </summary>
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GlobalSearch(
        [FromQuery] string? q = null,
        [FromQuery] int limit = 5,
        CancellationToken ct = default)
    {
        var userId = GetUserIdFromToken();
        var postsAuthResult = await authorizationService.AuthorizeAsync(User, "Permission:posts.read:all");
        var usersAuthResult = await authorizationService.AuthorizeAsync(User, "Permission:users.read:all");

        var result = await searchAllUseCase.ExecuteAsync(
            hasPostsPermission: postsAuthResult.Succeeded,
            hasUsersPermission: usersAuthResult.Succeeded,
            searchQuery: q,
            limit: limit,
            currentUserId: userId,
            ct: ct);

        return Ok(result);
    }

    /// <summary>
    /// Search posts with advanced filtering, sorting, and pagination.
    /// </summary>
    [HttpGet("posts")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> SearchPosts(
        [FromQuery] string? q = null,
        [FromQuery] Guid? subjectId = null,
        [FromQuery] string? tag = null,
        [FromQuery] string? sortBy = "relevance",
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken ct = default)
    {
        var userId = GetUserIdFromToken();
        var authResult = await authorizationService.AuthorizeAsync(User, "Permission:posts.read:all");

        var result = await searchPostsUseCase.ExecuteAsync(
            hasPermission: authResult.Succeeded,
            searchQuery: q,
            subjectId: subjectId,
            tag: tag,
            sortBy: sortBy,
            page: page,
            pageSize: pageSize,
            currentUserId: userId,
            ct: ct);

        return Ok(result);
    }

    /// <summary>
    /// Search users with pagination.
    /// </summary>
    [HttpGet("users")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> SearchUsers(
        [FromQuery] string? q = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken ct = default)
    {
        var authResult = await authorizationService.AuthorizeAsync(User, "Permission:users.read:all");

        var result = await searchUsersUseCase.ExecuteAsync(
            hasPermission: authResult.Succeeded,
            searchQuery: q,
            page: page,
            pageSize: pageSize,
            ct: ct);

        return Ok(result);
    }
}
