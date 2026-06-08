using Anonwork.Application.Features.AnonImages;
using Anonwork.Application.Features.AnonImages.DTOs.Requests;
using Anonwork.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Anonwork.API.Controllers;

[ApiController]
[Route("api/v1/anon-images")]
[Authorize]
public class AnonImagesController(
    GetAllAnonImagesUseCase getAllAnonImagesUseCase,
    GetAnonImageByIdUseCase getAnonImageByIdUseCase,
    CreateAnonImageUseCase createAnonImageUseCase,
    UpdateAnonImageUseCase updateAnonImageUseCase,
    DeleteAnonImageUseCase deleteAnonImageUseCase) : BaseApiController
{
    [HttpGet]
    [Authorize(Policy = "Permission:anon-images.read")]
    public async Task<IActionResult> GetAll(
        [FromQuery] bool? isActive = null,
        CancellationToken ct = default)
    {
        var result = await getAllAnonImagesUseCase.ExecuteAsync(isActive, ct);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [Authorize(Policy = "Permission:anon-images.read")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        try
        {
            var result = await getAnonImageByIdUseCase.ExecuteAsync(id, ct);
            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpPost]
    [Authorize(Policy = "Permission:anon-images.create")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> Create(
        [FromForm] CreateAnonImageRequestDto request,
        CancellationToken ct = default)
    {
        var result = await createAnonImageUseCase.ExecuteAsync(request, ct);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Policy = "Permission:anon-images.update")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> Update(
        Guid id,
        [FromForm] UpdateAnonImageRequestDto request,
        CancellationToken ct)
    {
        var result = await updateAnonImageUseCase.ExecuteAsync(id, request, ct);
        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Policy = "Permission:anon-images.delete")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await deleteAnonImageUseCase.ExecuteAsync(id, ct);
        return NoContent();
    }
}
