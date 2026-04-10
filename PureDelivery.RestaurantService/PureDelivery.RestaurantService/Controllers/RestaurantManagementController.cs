using Microsoft.AspNetCore.Mvc;
using PureDelivery.Shared.Contracts.Domain.Models;
using Restaurant.Application.DTOs.RestaurantManagement;
using Restaurant.Application.Services;

namespace PureDelivery.RestaurantService.Controllers
{
    /// <summary>
    /// Редактирование информации о ресторане — только для менеджеров.
    /// </summary>
    [ApiController]
    [Route("api/v1/restaurant/[controller]")]
    [Produces("application/json")]
    public class RestaurantManagementController : ControllerBase
    {
        private readonly IRestaurantManagementService _managementService;
        private readonly ILogger<RestaurantManagementController> _logger;

        public RestaurantManagementController(
            IRestaurantManagementService managementService,
            ILogger<RestaurantManagementController> logger)
        {
            _managementService = managementService;
            _logger = logger;
        }

        /// <summary>
        /// Получить полную информацию о ресторане для панели менеджера.
        /// </summary>
        [HttpGet("{restaurantId:guid}")]
        [ProducesResponseType(typeof(BaseResponse<RestaurantManagementDetailDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<RestaurantManagementDetailDto>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<BaseResponse<RestaurantManagementDetailDto>>> GetRestaurant(
            [FromRoute] Guid restaurantId, CancellationToken ct = default)
        {
            var result = await _managementService.GetRestaurantAsync(restaurantId, ct);
            if (!result.IsSuccess) return NotFound(result);
            return Ok(result);
        }

        /// <summary>
        /// Обновить базовую информацию: название, описание, картинка, тип кухни, теги.
        /// </summary>
        [HttpPut("{restaurantId:guid}/info")]
        [ProducesResponseType(typeof(BaseResponse<RestaurantManagementDetailDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<RestaurantManagementDetailDto>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<BaseResponse<RestaurantManagementDetailDto>>> UpdateInfo(
            [FromRoute] Guid restaurantId,
            [FromBody] UpdateRestaurantInfoRequest request,
            CancellationToken ct = default)
        {
            var result = await _managementService.UpdateInfoAsync(restaurantId, request, ct);
            if (!result.IsSuccess) return NotFound(result);
            return Ok(result);
        }

        /// <summary>
        /// Обновить настройки: контакты, мин. сумма заказа, время приготовления.
        /// </summary>
        [HttpPut("{restaurantId:guid}/settings")]
        [ProducesResponseType(typeof(BaseResponse<RestaurantSettingsManagementDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<RestaurantSettingsManagementDto>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<BaseResponse<RestaurantSettingsManagementDto>>> UpdateSettings(
            [FromRoute] Guid restaurantId,
            [FromBody] UpdateRestaurantSettingsRequest request,
            CancellationToken ct = default)
        {
            if (!ModelState.IsValid)
                return BadRequest(BaseResponse<RestaurantSettingsManagementDto>.Failure("Invalid request data"));

            var result = await _managementService.UpdateSettingsAsync(restaurantId, request, ct);
            if (!result.IsSuccess) return NotFound(result);
            return Ok(result);
        }

        /// <summary>
        /// Обновить график работы (полная замена всех дней).
        /// </summary>
        [HttpPut("{restaurantId:guid}/working-hours")]
        [ProducesResponseType(typeof(BaseResponse<List<WorkingHoursManagementDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<List<WorkingHoursManagementDto>>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<BaseResponse<List<WorkingHoursManagementDto>>>> UpsertWorkingHours(
            [FromRoute] Guid restaurantId,
            [FromBody] UpsertWorkingHoursRequest request,
            CancellationToken ct = default)
        {
            if (!ModelState.IsValid)
                return BadRequest(BaseResponse<List<WorkingHoursManagementDto>>.Failure("Invalid request data"));

            var result = await _managementService.UpsertWorkingHoursAsync(restaurantId, request, ct);
            if (!result.IsSuccess) return BadRequest(result);
            return Ok(result);
        }
    }
}
