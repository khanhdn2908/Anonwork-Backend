using Anonwork.Application.Features.SubscriptionPlans;
using Anonwork.Application.Features.SubscriptionPlans.DTOs.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Anonwork.API.Controllers;

[ApiController]
[Route("api/v1/subscription-plans")]
[Authorize]
public class SubscriptionPlansController(
    GetAllSubscriptionPlansUseCase getAllSubscriptionPlansUseCase,
    GetSubscriptionPlanByIdUseCase getSubscriptionPlanByIdUseCase,
    GetSubscriptionPlanBySlugUseCase getSubscriptionPlanBySlugUseCase,
    CreateSubscriptionPlanUseCase createSubscriptionPlanUseCase,
    UpdateSubscriptionPlanUseCase updateSubscriptionPlanUseCase,
    DeleteSubscriptionPlanUseCase deleteSubscriptionPlanUseCase) : BaseApiController
{

    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? searchTerm = null,
        [FromQuery] bool? isActive = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken ct = default)
    {
        var request = new GetAllSubscriptionPlansRequestDto(searchTerm, isActive, page, pageSize);
        var result = await getAllSubscriptionPlansUseCase.ExecuteAsync(request, ct);
        return Ok(result);
    }

  
    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct = default)
    {
        try
        {
            var result = await getSubscriptionPlanByIdUseCase.ExecuteAsync(id, ct);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpGet("slug/{slug}")]
    [Authorize(Policy = "Permission:subscription-plans.read")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetBySlug(string slug, CancellationToken ct = default)
    {
        try
        {
            var result = await getSubscriptionPlanBySlugUseCase.ExecuteAsync(slug, ct);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

 
    [HttpPost]
    [Authorize(Policy = "Permission:subscription-plans.create")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create(
        [FromBody] CreateSubscriptionPlanRequestDto request,
        CancellationToken ct = default)
    {
        try
        {
            var result = await createSubscriptionPlanUseCase.ExecuteAsync(request, ct);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

   
    [HttpPut("{id:guid}")]
    [Authorize(Policy = "Permission:subscription-plans.update")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateSubscriptionPlanRequestDto request,
        CancellationToken ct = default)
    {
        try
        {
            var result = await updateSubscriptionPlanUseCase.ExecuteAsync(id, request, ct);
            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Policy = "Permission:subscription-plans.delete")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct = default)
    {
        try
        {
            await deleteSubscriptionPlanUseCase.ExecuteAsync(id, ct);
            return NoContent();
        }
        catch (Exception ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }
}