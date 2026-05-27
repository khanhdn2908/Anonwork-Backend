using Anonwork.Application.Features.UserSubscriptions;
using Anonwork.Application.Features.UserSubscriptions.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Anonwork.API.Controllers;

[ApiController]
[Route("api/v1/user-subscriptions")]
[Authorize]
public class UserSubscriptionsController(
    CreateUserSubscriptionUseCase createUserSubscriptionUseCase,
    GetUserSubscriptionByIdUseCase getUserSubscriptionByIdUseCase,
    GetUserSubscriptionsByUserIdUseCase getUserSubscriptionsByUserIdUseCase,
    UpdateUserSubscriptionUseCase updateUserSubscriptionUseCase,
    DeleteUserSubscriptionUseCase deleteUserSubscriptionUseCase) : BaseApiController
{
    /// <summary>
    /// Create a new user subscription
    /// </summary>
    /// <remarks>
    /// Creates a new subscription for a user with the specified plan and order.
    /// Automatically calculates expiration date based on plan duration.
    /// 
    /// Sample request:
    /// 
    ///     POST /api/v1/user-subscriptions
    ///     {
    ///         "userId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    ///         "planId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    ///         "orderId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    ///         "status": "Active"
    ///     }
    /// </remarks>
    /// <param name="request">Subscription creation data</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Created subscription details</returns>
    /// <response code="201">Subscription created successfully</response>
    /// <response code="400">Invalid request data or business rule violation</response>
    /// <response code="404">User or subscription plan not found</response>
    [Authorize(Roles = "admin")]
    [HttpPost]
    [ProducesResponseType(typeof(UserSubscriptionResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Create(
        [FromBody] CreateUserSubscriptionRequestDto request,
        CancellationToken ct = default)
    {
        try
        {
            var result = await createUserSubscriptionUseCase.ExecuteAsync(request, ct);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }
        catch (ArgumentException ex)
        {
            return NotFound(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Get user subscription by ID
    /// </summary>
    /// <remarks>
    /// Retrieves a specific user subscription by its unique identifier.
    /// Includes user and plan information in the response.
    /// 
    /// Sample request:
    /// 
    ///     GET /api/v1/user-subscriptions/3fa85f64-5717-4562-b3fc-2c963f66afa6
    /// </remarks>
    /// <param name="id">Subscription unique identifier</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Subscription details</returns>
    /// <response code="200">Subscription found and returned</response>
    /// <response code="404">Subscription not found</response>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(UserSubscriptionResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct = default)
    {
        var result = await getUserSubscriptionByIdUseCase.ExecuteAsync(id, ct);
        if (result == null)
            return NotFound($"User subscription with ID {id} not found");

        return Ok(result);
    }

    /// <summary>
    /// Get all subscriptions for a specific user
    /// </summary>
    /// <remarks>
    /// Retrieves a paginated list of subscriptions for a specific user.
    /// Results are ordered by creation date (newest first).
    /// 
    /// Sample request:
    /// 
    ///     GET /api/v1/user-subscriptions/user/3fa85f64-5717-4562-b3fc-2c963f66afa6?page=1&pageSize=10
    /// </remarks>
    /// <param name="userId">User unique identifier</param>
    /// <param name="page">Page number (default: 1)</param>
    /// <param name="pageSize">Items per page (default: 10, max: 100)</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Paginated list of user subscriptions</returns>
    /// <response code="200">Subscriptions retrieved successfully</response>
    /// <response code="404">User not found</response>
    [HttpGet("user/{userId:guid}")]
    [ProducesResponseType(typeof(UserSubscriptionListPaginatedResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByUserId(
        Guid userId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken ct = default)
    {
        try
        {
            // Validate page size
            if (pageSize > 100) pageSize = 100;
            if (pageSize < 1) pageSize = 10;
            if (page < 1) page = 1;

            var request = new GetUserSubscriptionsByUserIdRequestDto(userId, page, pageSize);
            var result = await getUserSubscriptionsByUserIdUseCase.ExecuteAsync(request, ct);
            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            return NotFound(ex.Message);
        }
    }

    /// <summary>
    /// Update user subscription
    /// </summary>
    /// <remarks>
    /// Updates specific fields of a user subscription. Only provided fields will be updated.
    /// Commonly used to change subscription status or extend expiration date.
    /// 
    /// Sample request:
    /// 
    ///     PUT /api/v1/user-subscriptions/3fa85f64-5717-4562-b3fc-2c963f66afa6
    ///     {
    ///         "status": "Cancelled",
    ///         "expiresAt": "2024-12-31T23:59:59Z"
    ///     }
    /// </remarks>
    /// <param name="id">Subscription unique identifier</param>
    /// <param name="request">Update data (partial)</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Updated subscription details</returns>
    /// <response code="200">Subscription updated successfully</response>
    /// <response code="404">Subscription not found</response>
    [Authorize(Roles = "admin")]
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(UserSubscriptionResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateUserSubscriptionRequestDto request,
        CancellationToken ct = default)
    {
        try
        {
            var result = await updateUserSubscriptionUseCase.ExecuteAsync(id, request, ct);
            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            return NotFound(ex.Message);
        }
    }

    /// <summary>
    /// Delete user subscription
    /// </summary>
    /// <remarks>
    /// Deletes a user subscription. Active subscriptions cannot be deleted directly - 
    /// they must be cancelled first using the update endpoint.
    /// 
    /// Sample request:
    /// 
    ///     DELETE /api/v1/user-subscriptions/3fa85f64-5717-4562-b3fc-2c963f66afa6
    /// </remarks>
    /// <param name="id">Subscription unique identifier</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Deletion result</returns>
    /// <response code="204">Subscription deleted successfully</response>
    /// <response code="400">Cannot delete active subscription</response>
    /// <response code="404">Subscription not found</response>
    [Authorize(Roles = "admin")]
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct = default)
    {
        try
        {
            var success = await deleteUserSubscriptionUseCase.ExecuteAsync(id, ct);
            if (!success)
                return NotFound($"User subscription with ID {id} not found");

            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}