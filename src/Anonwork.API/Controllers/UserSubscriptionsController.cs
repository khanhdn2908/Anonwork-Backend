using Anonwork.Application.Features.UserSubscriptions;
using Anonwork.Application.Features.UserSubscriptions.DTOs.Requests;
using Anonwork.Application.Features.UserSubscriptions.DTOs.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Anonwork.API.Controllers;

[ApiController]
[Route("api/v1/user-subscriptions")]
[Authorize]
public class UserSubscriptionsController(
    //CreateUserSubscriptionUseCase createUserSubscriptionUseCase,
    GetUserSubscriptionByIdUseCase getUserSubscriptionByIdUseCase,
    GetUserSubscriptionsByUserIdUseCase getUserSubscriptionsByUserIdUseCase
    //UpdateUserSubscriptionUseCase updateUserSubscriptionUseCase,
    //DeleteUserSubscriptionUseCase deleteUserSubscriptionUseCase
    ) : BaseApiController
{

    //[HttpPost]
    //[ProducesResponseType(typeof(UserSubscriptionResponseDto), StatusCodes.Status201Created)]
    //[ProducesResponseType(StatusCodes.Status400BadRequest)]
    //[ProducesResponseType(StatusCodes.Status404NotFound)]
    //public async Task<IActionResult> Create(
    //    [FromBody] CreateUserSubscriptionRequestDto request,
    //    CancellationToken ct = default)
    //{
    //    try
    //    {
    //        var result = await createUserSubscriptionUseCase.ExecuteAsync(request, ct);
    //        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    //    }
    //    catch (ArgumentException ex)
    //    {
    //        return NotFound(ex.Message);
    //    }
    //    catch (InvalidOperationException ex)
    //    {
    //        return BadRequest(ex.Message);
    //    }
    //}


    [HttpGet("{id:guid}")]
    [Authorize(Policy = "Permission:user-subscriptions.read")]
    [ProducesResponseType(typeof(UserSubscriptionResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct = default)
    {
        var result = await getUserSubscriptionByIdUseCase.ExecuteAsync(id, ct);
        if (result == null)
            return NotFound($"User subscription with ID {id} not found");

        return Ok(result);
    }

  
    [HttpGet("user/{userId:guid}")]
    [Authorize(Policy = "Permission:user-subscriptions.read")]
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

  
    //[Authorize(Policy = "Permission:user-subscriptions.manage")]
    //[HttpPut("{id:guid}")]
    //[ProducesResponseType(typeof(UserSubscriptionResponseDto), StatusCodes.Status200OK)]
    //[ProducesResponseType(StatusCodes.Status404NotFound)]
    //public async Task<IActionResult> Update(
    //    Guid id,
    //    [FromBody] UpdateUserSubscriptionRequestDto request,
    //    CancellationToken ct = default)
    //{
    //    try
    //    {
    //        var result = await updateUserSubscriptionUseCase.ExecuteAsync(id, request, ct);
    //        return Ok(result);
    //    }
    //    catch (ArgumentException ex)
    //    {
    //        return NotFound(ex.Message);
    //    }
    //}

  
    //[Authorize(Policy = "Permission:user-subscriptions.manage")]
    //[HttpDelete("{id:guid}")]
    //[ProducesResponseType(StatusCodes.Status204NoContent)]
    //[ProducesResponseType(StatusCodes.Status400BadRequest)]
    //[ProducesResponseType(StatusCodes.Status404NotFound)]
    //public async Task<IActionResult> Delete(Guid id, CancellationToken ct = default)
    //{
    //    try
    //    {
    //        var success = await deleteUserSubscriptionUseCase.ExecuteAsync(id, ct);
    //        if (!success)
    //            return NotFound($"User subscription with ID {id} not found");

    //        return NoContent();
    //    }
    //    catch (InvalidOperationException ex)
    //    {
    //        return BadRequest(ex.Message);
    //    }
    //}
}