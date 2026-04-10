using Microsoft.AspNetCore.Mvc;
using PureDelivery.Shared.Contracts.Domain.Models;
using Restaurant.Application.DTOs;
using Restaurant.Application.Services;

namespace PureDelivery.RestaurantService.Controllers
{
    [ApiController]
    [Route("api/v1/restaurant/[controller]")]
    [Produces("application/json")]
    public class RestaurantManagerController : ControllerBase
    {
        private readonly IRestaurantManagerService _managerService;
        private readonly ILogger<RestaurantManagerController> _logger;

        public RestaurantManagerController(
            IRestaurantManagerService managerService,
            ILogger<RestaurantManagerController> logger)
        {
            _managerService = managerService;
            _logger = logger;
        }

        /// <summary>
        /// Создать менеджера ресторана (учётные данные + профиль).
        /// Пароль будет храниться в IdentityService, профиль — здесь.
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(BaseResponse<ManagerDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(BaseResponse<ManagerDto>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(BaseResponse<ManagerDto>), StatusCodes.Status409Conflict)]
        public async Task<ActionResult<BaseResponse<ManagerDto>>> Create(
            [FromBody] CreateManagerRequest request,
            CancellationToken ct = default)
        {
            if (!ModelState.IsValid)
                return BadRequest(BaseResponse<ManagerDto>.Failure("Invalid request data"));

            var result = await _managerService.CreateManagerAsync(request, ct);

            if (!result.IsSuccess)
            {
                if (result.Error?.Contains("already exists") == true)
                    return Conflict(result);
                return BadRequest(result);
            }

            return CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result);
        }

        /// <summary>
        /// Получить профиль менеджера по его локальному Id.
        /// </summary>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(BaseResponse<ManagerDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<ManagerDto>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<BaseResponse<ManagerDto>>> GetById(
            [FromRoute] Guid id, CancellationToken ct = default)
        {
            var result = await _managerService.GetByIdAsync(id, ct);
            if (!result.IsSuccess) return NotFound(result);
            return Ok(result);
        }

        /// <summary>
        /// Получить профиль менеджера по UserId (возвращает IdentityService при логине).
        /// Используется после логина: AuthDto.customerId → GET /manager/by-user/{userId}
        /// </summary>
        [HttpGet("by-user/{userId:guid}")]
        [ProducesResponseType(typeof(BaseResponse<ManagerDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<ManagerDto>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<BaseResponse<ManagerDto>>> GetByUserId(
            [FromRoute] Guid userId, CancellationToken ct = default)
        {
            var result = await _managerService.GetByUserIdAsync(userId, ct);
            if (!result.IsSuccess) return NotFound(result);
            return Ok(result);
        }

        /// <summary>
        /// Список менеджеров ресторана.
        /// </summary>
        [HttpGet("restaurant/{restaurantId:guid}")]
        [ProducesResponseType(typeof(BaseResponse<List<ManagerDto>>), StatusCodes.Status200OK)]
        public async Task<ActionResult<BaseResponse<List<ManagerDto>>>> GetByRestaurant(
            [FromRoute] Guid restaurantId, CancellationToken ct = default)
        {
            var result = await _managerService.GetByRestaurantIdAsync(restaurantId, ct);
            return Ok(result);
        }

        /// <summary>
        /// Деактивировать менеджера.
        /// </summary>
        [HttpDelete("{id:guid}")]
        [ProducesResponseType(typeof(BaseResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<bool>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<BaseResponse<bool>>> Deactivate(
            [FromRoute] Guid id, CancellationToken ct = default)
        {
            var result = await _managerService.DeactivateAsync(id, ct);
            if (!result.IsSuccess) return NotFound(result);
            return Ok(result);
        }
    }
}
