using Anonwork.Application.Features.Posts;
using Anonwork.Application.Features.Posts.DTOs;
using Anonwork.Application.Features.Subjects;
using Anonwork.Application.Features.Subjects.DTOs.Requests;
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
  
    [HttpGet]
    [Authorize(Policy = "Permission:subjects.read")]
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

   
    [HttpGet("{id:guid}")]
    [Authorize(Policy = "Permission:subjects.read")]
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
            var result = await getPostsBySubjectUseCase.ExecuteAsync(subjectId, page, pageSize, null,ct);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }
}
