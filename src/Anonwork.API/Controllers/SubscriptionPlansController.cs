using Anonwork.Application.Features.SubscriptionPlans;
using Anonwork.Application.Features.SubscriptionPlans.DTOs;
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
    /// <summary>
    /// Get all subscription plans with search and pagination
    /// </summary>
    /// <remarks>
    /// Retrieves a paginated list of subscription plans, sorted by price (ascending).
    /// Supports search by name or slug and filtering by active status.
    /// 
    /// Sample request:
    /// 
    ///     GET /api/v1/subscription-plans?searchTerm=premium&isActive=true&page=1&pageSize=10
    /// </remarks>
    /// <param name="searchTerm">Search query (optional, searches in name and slug)</param>
    /// <param name="isActive">Filter by active status (optional)</param>
    /// <param name="page">Page number (default: 1)</param>
    /// <param name="pageSize">Items per page (default: 10, max: 100)</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Paginated list of subscription plans</returns>
    /// <response code="200">Subscription plans retrieved successfully</response>
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

    /// <summary>
    /// Get a subscription plan by ID
    /// </summary>
    /// <remarks>
    /// Retrieves a specific subscription plan by its ID.
    /// 
    /// Sample request:
    /// 
    ///     GET /api/v1/subscription-plans/550e8400-e29b-41d4-a716-446655440000
    /// </remarks>
    /// <param name="id">Subscription plan ID</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Subscription plan details</returns>
    /// <response code="200">Subscription plan found</response>
    /// <response code="404">Subscription plan not found</response>
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

    /// <summary>
    /// Get a subscription plan by slug
    /// </summary>
    /// <remarks>
    /// Retrieves a specific subscription plan by its slug.
    /// 
    /// Sample request:
    /// 
    ///     GET /api/v1/subscription-plans/slug/premium-monthly
    /// </remarks>
    /// <param name="slug">Subscription plan slug</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Subscription plan details</returns>
    /// <response code="200">Subscription plan found</response>
    /// <response code="404">Subscription plan not found</response>
    [HttpGet("slug/{slug}")]
    [AllowAnonymous]
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

    /// <summary>
    /// Create a new subscription plan
    /// </summary>
    /// <remarks>
    /// Creates a new subscription plan. Requires authentication and admin role.
    /// 
    /// Sample request:
    /// 
    ///     POST /api/v1/subscription-plans
    ///     {
    ///       "name": "Premium Monthly",
    ///       "slug": "premium-monthly",
    ///       "price": 999,
    ///       "durationDays": 30,
    ///       "features": "Unlimited posts, Priority support",
    ///       "isActive": true
    ///     }
    /// </remarks>
    /// <param name="request">Subscription plan creation request</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Created subscription plan with 201 status</returns>
    /// <response code="201">Subscription plan created successfully</response>
    /// <response code="400">Invalid request data</response>
    /// <response code="401">Unauthorized</response>
    /// <response code="409">Subscription plan with slug already exists</response>
    [Authorize(Roles = "admin")]
    [HttpPost]
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

    /// <summary>
    /// Update a subscription plan
    /// </summary>
    /// <remarks>
    /// Updates an existing subscription plan. Requires authentication and admin role.
    /// 
    /// Sample request:
    /// 
    ///     PUT /api/v1/subscription-plans/550e8400-e29b-41d4-a716-446655440000
    ///     {
    ///       "name": "Premium Monthly Updated",
    ///       "slug": "premium-monthly-updated",
    ///       "price": 1299,
    ///       "durationDays": 30,
    ///       "features": "Unlimited posts, Priority support, Advanced analytics",
    ///       "isActive": true
    ///     }
    /// </remarks>
    /// <param name="id">Subscription plan ID</param>
    /// <param name="request">Subscription plan update request</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Updated subscription plan</returns>
    /// <response code="200">Subscription plan updated successfully</response>
    /// <response code="400">Invalid request data</response>
    /// <response code="401">Unauthorized</response>
    /// <response code="404">Subscription plan not found</response>
    /// <response code="409">Subscription plan with slug already exists</response>
    [Authorize(Roles = "admin")]
    [HttpPut("{id:guid}")]
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

    /// <summary>
    /// Delete a subscription plan
    /// </summary>
    /// <remarks>
    /// Deletes a subscription plan. Requires authentication and admin role.
    /// 
    /// Sample request:
    /// 
    ///     DELETE /api/v1/subscription-plans/550e8400-e29b-41d4-a716-446655440000
    /// </remarks>
    /// <param name="id">Subscription plan ID</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>No content</returns>
    /// <response code="204">Subscription plan deleted successfully</response>
    /// <response code="401">Unauthorized</response>
    /// <response code="404">Subscription plan not found</response>
    [Authorize(Roles = "admin")]
    [HttpDelete("{id:guid}")]
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