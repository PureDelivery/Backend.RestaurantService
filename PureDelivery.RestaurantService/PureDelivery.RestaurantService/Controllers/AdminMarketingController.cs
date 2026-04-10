using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PureDelivery.Shared.Contracts.Domain.Models;
using Restaurant.Application.DTOs.AdminMarketing;
using Restaurant.Application.Services;

namespace PureDelivery.RestaurantService.Controllers
{
    /// <summary>
    /// Управление промоушнами ресторанов — только для администраторов PureDelivery.
    /// </summary>
    [ApiController]
    [Route("api/v1/restaurant/[controller]")]
    [Produces("application/json")]
    public class AdminMarketingController : ControllerBase
    {
        private readonly IAdminMarketingService _adminMarketingService;
        private readonly ILogger<AdminMarketingController> _logger;

        public AdminMarketingController(
            IAdminMarketingService adminMarketingService,
            ILogger<AdminMarketingController> logger)
        {
            _adminMarketingService = adminMarketingService;
            _logger = logger;
        }

        /// <summary>
        /// Получить маркетинговую информацию конкретного ресторана.
        /// </summary>
        [HttpGet("{restaurantId:guid}")]
        [Authorize(Roles = "Manager, Admin")]
        [ProducesResponseType(typeof(BaseResponse<AdminMarketingDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<AdminMarketingDto>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<BaseResponse<AdminMarketingDto>>> GetMarketing(
            [FromRoute] Guid restaurantId, CancellationToken ct = default)
        {
            var result = await _adminMarketingService.GetMarketingAsync(restaurantId, ct);
            if (!result.IsSuccess) return NotFound(result);
            return Ok(result);
        }

        /// <summary>
        /// Обновить маркетинговые настройки ресторана:
        /// IsFeatured, SearchPriority (1–10), AllowsPromotions, AccountManager.
        /// </summary>
        [Authorize(Roles = "Manager")]
        [HttpPut("{restaurantId:guid}")]
        [ProducesResponseType(typeof(BaseResponse<AdminMarketingDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<AdminMarketingDto>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<BaseResponse<AdminMarketingDto>>> UpdateMarketing(
            [FromRoute] Guid restaurantId,
            [FromBody] UpdateRestaurantMarketingRequest request,
            CancellationToken ct = default)
        {
            if (!ModelState.IsValid)
                return BadRequest(BaseResponse<AdminMarketingDto>.Failure("Invalid request data"));

            var result = await _adminMarketingService.UpdateMarketingAsync(restaurantId, request, ct);
            if (!result.IsSuccess) return BadRequest(result);
            return Ok(result);
        }

        /// <summary>
        /// Получить все рестораны с активным featured (для панели администратора).
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpGet("featured")]
        [ProducesResponseType(typeof(BaseResponse<List<AdminMarketingDto>>), StatusCodes.Status200OK)]
        public async Task<ActionResult<BaseResponse<List<AdminMarketingDto>>>> GetFeatured(
            CancellationToken ct = default)
        {
            var result = await _adminMarketingService.GetAllFeaturedAsync(ct);
            return Ok(result);
        }
    }
}
