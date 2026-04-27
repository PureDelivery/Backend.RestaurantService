using Microsoft.AspNetCore.Mvc;
using PureDelivery.Shared.Contracts.Domain.Models;
using Restaurant.Application.DTOs.Reviews;
using Restaurant.Application.Services;

namespace PureDelivery.RestaurantService.Controllers;

[ApiController]
[Route("api/v1/restaurant/[controller]")]
[Produces("application/json")]
public class RestaurantReviewController(
    IRestaurantReviewService reviewService,
    ILogger<RestaurantReviewController> logger) : ControllerBase
{
    /// <summary>
    /// Customer submits a review for a restaurant after a completed order.
    /// </summary>
    [HttpPost("{restaurantId:guid}")]
    [ProducesResponseType(typeof(BaseResponse<RestaurantReviewDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(BaseResponse<RestaurantReviewDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(BaseResponse<RestaurantReviewDto>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(BaseResponse<RestaurantReviewDto>), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<BaseResponse<RestaurantReviewDto>>> SubmitReview(
        [FromRoute] Guid restaurantId,
        [FromBody] SubmitReviewRequest request,
        CancellationToken ct = default)
    {
        if (!ModelState.IsValid)
            return BadRequest(BaseResponse<RestaurantReviewDto>.Failure("Invalid request data."));

        var result = await reviewService.SubmitReviewAsync(restaurantId, request, ct);

        if (!result.IsSuccess)
        {
            if (result.Error?.Contains("already reviewed") == true) return Conflict(result);
            if (result.Error?.Contains("not found") == true)        return NotFound(result);
            return BadRequest(result);
        }

        return StatusCode(StatusCodes.Status201Created, result);
    }

    /// <summary>
    /// Get paginated reviews for a restaurant with average rating.
    /// </summary>
    [HttpGet("{restaurantId:guid}")]
    [ProducesResponseType(typeof(BaseResponse<PagedReviewsDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse<PagedReviewsDto>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BaseResponse<PagedReviewsDto>>> GetReviews(
        [FromRoute] Guid restaurantId,
        [FromQuery] int page     = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken ct     = default)
    {
        var result = await reviewService.GetReviewsAsync(restaurantId, page, pageSize, ct);
        if (!result.IsSuccess) return NotFound(result);
        return Ok(result);
    }
}
