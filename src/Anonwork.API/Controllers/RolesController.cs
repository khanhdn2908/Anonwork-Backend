using Anonwork.Application.Features.Roles;
using Anonwork.Application.Features.Roles.DTOs.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Anonwork.API.Controllers;

[ApiController]
[Route("api/v1/roles")]

public class RolesController(
    GetAllRolesUseCase getAllRolesUseCase,
    GetRoleByIdUseCase getRoleByIdUseCase,
    CreateRoleUseCase createRoleUseCase,
    UpdateRoleUseCase updateRoleUseCase,
    DeleteRoleUseCase deleteRoleUseCase,
    DeleteRoleUseCasePermanent deleteRoleUseCasePermanent,
    AssignPermissionToRoleUseCase assignPermissionToRoleUseCase,
    AssignPermissionsToRoleUseCase assignPermissionsToRoleUseCase,
    RemovePermissionFromRoleUseCase removePermissionFromRoleUseCase,
    GetRolePermissionsUseCase getRolePermissionsUseCase) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct = default)
        => Ok(await getAllRolesUseCase.ExecuteAsync(ct));

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct = default)
    {
        try
        {
            return Ok(await getRoleByIdUseCase.ExecuteAsync(id, ct));
        }
        catch (Exception ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpPost]
    [Authorize(Policy = "Permission:roles.create")]
    public async Task<IActionResult> Create([FromBody] RoleRequestDto request, CancellationToken ct = default)
    {
        try
        {
            var result = await createRoleUseCase.ExecuteAsync(request, ct);
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
    [Authorize(Policy = "Permission:roles.update")]
    public async Task<IActionResult> Update(Guid id, [FromBody] RoleRequestDto request, CancellationToken ct = default)
    {
        try
        {
            return Ok(await updateRoleUseCase.ExecuteAsync(id, request, ct));
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
    [Authorize(Policy = "Permission:roles.delete")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct = default)
    {
        try
        {
            await deleteRoleUseCase.ExecuteAsync(id, ct);
            return NoContent();
        }
        catch (Exception ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpDelete("{id:guid}/permanent")]
    [Authorize(Policy = "Permission:roles.delete-permanent")]
    public async Task<IActionResult> DeletePermanent(Guid id, CancellationToken ct = default)
    {
        try
        {
            await deleteRoleUseCasePermanent.ExecuteAsync(id, ct);
            return NoContent();
        }
        catch (Exception ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpGet("{roleId:guid}/permissions")]
    [Authorize(Policy = "Permission:roles.read-permissions")]
    public async Task<IActionResult> GetPermissions(Guid roleId, CancellationToken ct = default)
    {
        try
        {
            return Ok(await getRolePermissionsUseCase.ExecuteAsync(roleId, ct));
        }
        catch (Exception ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpPost("{roleId:guid}/permissions/{permissionId:guid}")]
    [Authorize(Policy = "Permission:roles.assign-permission")]
    public async Task<IActionResult> AssignPermission(Guid roleId, Guid permissionId, CancellationToken ct = default)
    {
        try
        {
            await assignPermissionToRoleUseCase.ExecuteAsync(roleId, permissionId, ct);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpPost("{roleId:guid}/permissions")]
    [Authorize(Policy = "Permission:roles.assign-permission")]
    public async Task<IActionResult> AssignManyPermissions(Guid roleId, [FromBody] AssignPermissionsRequestDto request, CancellationToken ct = default)
    {
        try
        {
            await assignPermissionsToRoleUseCase.ExecuteAsync(roleId, request, ct);
            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpDelete("{roleId:guid}/permissions/{permissionId:guid}")]
    [Authorize(Policy = "Permission:roles.remove-permission")]
    public async Task<IActionResult> RemovePermission(Guid roleId, Guid permissionId, CancellationToken ct = default)
    {
        try
        {
            await removePermissionFromRoleUseCase.ExecuteAsync(roleId, permissionId, ct);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }
}
