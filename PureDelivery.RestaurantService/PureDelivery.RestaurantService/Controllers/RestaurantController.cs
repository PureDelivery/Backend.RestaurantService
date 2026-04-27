using Microsoft.AspNetCore.Mvc;
using PureDelivery.Shared.Contracts.Common;
using PureDelivery.Shared.Contracts.Domain.Models;
using PureDelivery.Shared.Contracts.DTOs.Restaurants.Requests;
using PureDelivery.Shared.Contracts.DTOs.Restaurants.Responses;
using Restaurant.Application.Services;
using Restaurant.Application.Exceptions;

namespace PureDelivery.RestaurantService.Controllers
{
    [ApiController]
    [Route("api/v1/restaurant/[controller]")]
    public class RestaurantController : ControllerBase
    {
        private readonly ILogger<RestaurantController> _logger;
        private readonly IRestaurantService _restaurantService;

        public RestaurantController(ILogger<RestaurantController> logger, IRestaurantService restaurantService)
        {
            _logger = logger;
            _restaurantService = restaurantService;
        }

        /// <summary>
        /// 1) ��������� ���� ���������� � ����������� �� �������
        /// </summary>
        [HttpPost("list")]
        public async Task<ActionResult<BaseResponse<PagedResult<RestaurantDto>>>> GetRestaurants(
            [FromBody] GetRestaurantsRequest request)
        {
            try
            {
                var restaurantsByLocation = await _restaurantService.GetRestaurantsByLocationAsync(request);

                return Ok(BaseResponse<PagedResult<RestaurantDto>>.Success(restaurantsByLocation));
            }
            catch (RestaurantException ex)
            {
                _logger.LogWarning(ex, "Restaurant service error: {ErrorCode}", ex.ErrorCode);

                return StatusCode((int)ex.StatusCode, BaseResponse<PagedResult<RestaurantDto>>.Failure(ex.UserMessage));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error getting restaurants");
                return StatusCode(500, BaseResponse<PagedResult<RestaurantDto>>.Failure("Internal server error"));
            }
        }

        /// <summary>
        /// 2) ��������� ������ ��������� ����� ��������
        /// </summary>
        [HttpPost("{restaurantId}")]
        public async Task<ActionResult<BaseResponse<RestaurantDetailDto>>> GetRestaurantDetail(
            Guid restaurantId, [FromBody] GetRestaurantsRequest request)
        {
            try
            {
                var restaurant = await _restaurantService.GetRestaurantDetailAsync(restaurantId, request);

                return Ok(BaseResponse<RestaurantDetailDto>.Success(restaurant));
            }
            catch (RestaurantException ex) when (ex.ErrorCode == "RESTAURANT_NOT_FOUND")
            {
                return NotFound(BaseResponse<RestaurantDetailDto>.Failure(ex.UserMessage));
            }
            catch (RestaurantException ex)
            {
                _logger.LogWarning(ex, "Restaurant service error: {ErrorCode}", ex.ErrorCode);

                return StatusCode((int)ex.StatusCode, BaseResponse<RestaurantDetailDto>.Failure(ex.UserMessage));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error getting restaurant detail for {RestaurantId}", restaurantId);
                return StatusCode(500, BaseResponse<RestaurantDetailDto>.Failure("Internal server error"));
            }
        }

        /// <summary>
        /// 3) ����� �� ��������� (��������)
        /// </summary>
        [HttpPost("search")]
        public async Task<ActionResult<BaseResponse<PagedResult<RestaurantDto>>>> SearchRestaurants(
            [FromBody] SearchRestaurantRequest request)
        {
            try
            {
                var results = await _restaurantService.SearchRestaurantsAsync(request);

                return Ok(BaseResponse<PagedResult<RestaurantDto>>.Success(results));
            }
            catch (RestaurantException ex)
            {
                _logger.LogWarning(ex, "Restaurant service error: {ErrorCode}", ex.ErrorCode);

                return StatusCode((int)ex.StatusCode, BaseResponse<PagedResult<RestaurantDto>>.Failure(ex.UserMessage));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error searching restaurants for: {Query}", request.Query);
                return StatusCode(500, BaseResponse<PagedResult<RestaurantDto>>.Failure("Internal server error"));
            }
        }

        /// <summary>
        /// 4) ����� �� ����� (��������) - ���������� �������� ��� ���� ��� �����
        /// </summary>
        [HttpPost("dishes/search")]
        public async Task<ActionResult<BaseResponse<PagedResult<RestaurantDto>>>> SearchDishes(
            [FromBody] SearchRestaurantByDishRequest request)
        {
            try
            {
                var results = await _restaurantService.SearchRestaurantsByDishAsync(request);

                return Ok(BaseResponse<PagedResult<RestaurantDto>>.Success(results));
            }
            catch (RestaurantException ex)
            {
                _logger.LogWarning(ex, "Restaurant service error: {ErrorCode}", ex.ErrorCode);

                return StatusCode((int)ex.StatusCode, BaseResponse<PagedResult<RestaurantDto>>.Failure(ex.UserMessage));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error searching dishes for: {Query}", request.Query);
                return StatusCode(500, BaseResponse<PagedResult<RestaurantDto>>.Failure("Internal server error"));
            }
        }

        /// <summary>
        /// 5) ������������� ���������� ���������� �� ����� �/��� �����
        /// ����� �������� ������ �����, ������ ����, ��� � �� � ������ ������������
        /// </summary>
        [HttpPost("filter")]
        public async Task<ActionResult<BaseResponse<PagedResult<RestaurantDto>>>> FilterRestaurants(
            [FromBody] FilterRestaurantsRequest request)
        {
            try
            {
                // ������� ��������� �� �����������
                if (request.Latitude == 0 || request.Longitude == 0)
                {
                    return BadRequest(BaseResponse<PagedResult<RestaurantDto>>.Failure(
                        "Location coordinates are required"));
                }

                if ((request.CuisineTypes?.Any() != true) && (request.Tags?.Any() != true))
                {
                    return BadRequest(BaseResponse<PagedResult<RestaurantDto>>.Failure(
                        "Please select at least one cuisine type or tag"));
                }

                var results = await _restaurantService.FilterRestaurantsAsync(request);

                return Ok(BaseResponse<PagedResult<RestaurantDto>>.Success(results));
            }
            catch (RestaurantException ex) when (ex.ErrorCode == "VALIDATION_ERROR")
            {
                return BadRequest(BaseResponse<PagedResult<RestaurantDto>>.Failure(ex.UserMessage));
            }
            catch (RestaurantException ex)
            {
                _logger.LogWarning(ex, "Restaurant service error: {ErrorCode}", ex.ErrorCode);

                return StatusCode((int)ex.StatusCode, BaseResponse<PagedResult<RestaurantDto>>.Failure(ex.UserMessage));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in restaurant filtering");
                return StatusCode(500, BaseResponse<PagedResult<RestaurantDto>>.Failure("Internal server error"));
            }
        }

        /// <summary>
        /// 6) IDs ресторанів що доставляють на дану локацію (для AI сервісу)
        /// </summary>
        [HttpPost("ids-by-location")]
        public async Task<ActionResult<BaseResponse<List<Guid>>>> GetRestaurantIdsByLocation(
            [FromBody] GetRestaurantsRequest request)
        {
            try
            {
                var ids = await _restaurantService.GetRestaurantIdsByLocationAsync((double)request.Latitude, (double)request.Longitude);
                return Ok(BaseResponse<List<Guid>>.Success(ids));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting restaurant IDs by location");
                return StatusCode(500, BaseResponse<List<Guid>>.Failure("Internal server error"));
            }
        }

        /// <summary>
        /// 7) ��������� ������� ���������� ��� ���������� �������� (Snapshot)
        /// </summary>
        [HttpGet("{restaurantId}/internal")]
        public async Task<ActionResult<BaseResponse<RestaurantDetailDto>>> GetRestaurantInternal(Guid restaurantId)
        {
            try
            {
                // �������� ���������� ����� ������� ��� ������ ���������
                var restaurant = await _restaurantService.GetRestaurantByIdAsync(restaurantId);
                return Ok(BaseResponse<RestaurantDetailDto>.Success(restaurant));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting internal restaurant info for {RestaurantId}", restaurantId);
                return StatusCode(500, BaseResponse<RestaurantDetailDto>.Failure("Internal error"));
            }
        }
    }
}