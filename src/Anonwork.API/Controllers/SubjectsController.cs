using Anonwork.Application.Features.Posts;
using Anonwork.Application.Features.Posts.DTOs;
using Anonwork.Application.Features.Subjects;
using Anonwork.Application.Features.Subjects.DTOs.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

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
    DeleteSubjectUseCasePermanent deleteSubjectUseCasePermanent,
    GetPostsBySubjectUseCase getPostsBySubjectUseCase,
    IAuthorizationService authorizationService) : BaseApiController
{
  
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? search = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken ct = default)
    {
        var authResult = await authorizationService.AuthorizeAsync(User, "Permission:subjects.read:all");

        var result = await getSubjectsUseCase.ExecuteAsync(authResult.Succeeded,search, page, pageSize, ct);
        return Ok(result);
    }

   
    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct = default)
    {
        var authResult = await authorizationService.AuthorizeAsync(User, "Permission:subjects.read:all");

        try
        {
            var result = await getSubjectByIdUseCase.ExecuteAsync(id, authResult.Succeeded,ct);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

 
    [HttpPost]
    [Authorize(Policy = "Permission:subjects.create")]
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


    [HttpPut("{id:guid}")]
    [Authorize(Policy = "Permission:subjects.update")]
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

    [HttpDelete("{id:guid}")]
    [Authorize(Policy = "Permission:subjects.delete")]
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

    [HttpDelete("{id:guid}/permanent")]
    [Authorize(Policy = "Permission:subjects.delete-permanent")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeletePermanent(Guid id, CancellationToken ct = default)
    {
        try
        {
            await deleteSubjectUseCasePermanent.ExecuteAsync(id, ct);
            return NoContent();
        }
        catch (Exception ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }


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
        var userId = GetUserIdFromToken();
        var authResult = await authorizationService.AuthorizeAsync(User, "Permission:posts.read:all");
        try
        {
            var result = await getPostsBySubjectUseCase.ExecuteAsync(authResult.Succeeded, subjectId, userId, page, pageSize, ct);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }
}
