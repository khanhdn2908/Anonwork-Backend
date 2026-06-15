using Anonwork.Application.Features.Permissions;
using Anonwork.Application.Features.Permissions.DTOs.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Anonwork.API.Controllers;

[ApiController]
[Route("api/v1/permissions")]
[Authorize]
public class PermissionsController(
    GetAllPermissionsUseCase getAllPermissionsUseCase,
    GetPermissionByIdUseCase getPermissionByIdUseCase,
    CreatePermissionUseCase createPermissionUseCase,
    UpdatePermissionUseCase updatePermissionUseCase,
    DeletePermissionUseCase deletePermissionUseCase,
    DeletePermissionUseCasePermanent deletePermissionUseCasePermanent) : ControllerBase
{
    [HttpGet]
    [Authorize(Policy = "Permission:permissions.read")]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? searchTerm = null,
        [FromQuery] bool? isActive = null,
        CancellationToken ct = default)
        => Ok(await getAllPermissionsUseCase.ExecuteAsync(searchTerm, isActive, ct));

    [HttpGet("{id:guid}")]
    [Authorize(Policy = "Permission:permissions.read")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct = default)
    {
        try
        {
            return Ok(await getPermissionByIdUseCase.ExecuteAsync(id, ct));
        }
        catch (Exception ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpPost]
    [Authorize(Policy = "Permission:permissions.create")]
    public async Task<IActionResult> Create([FromBody] PermissionRequestDto request, CancellationToken ct = default)
    {
        try
        {
            var result = await createPermissionUseCase.ExecuteAsync(request, ct);
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
    [Authorize(Policy = "Permission:permissions.update")]
    public async Task<IActionResult> Update(Guid id, [FromBody] PermissionRequestDto request, CancellationToken ct = default)
    {
        try
        {
            return Ok(await updatePermissionUseCase.ExecuteAsync(id, request, ct));
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
    [Authorize(Policy = "Permission:permissions.delete")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct = default)
    {
        try
        {
            await deletePermissionUseCase.ExecuteAsync(id, ct);
            return NoContent();
        }
        catch (Exception ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpDelete("{id:guid}/permanent")]
    [Authorize(Policy = "Permission:permissions.delete-permanent")]
    public async Task<IActionResult> DeletePermanent(Guid id, CancellationToken ct = default)
    {
        try
        {
            await deletePermissionUseCasePermanent.ExecuteAsync(id, ct);
            return NoContent();
        }
        catch (Exception ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }
}
