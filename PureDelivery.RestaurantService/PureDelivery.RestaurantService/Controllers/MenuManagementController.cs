using Microsoft.AspNetCore.Mvc;
using PureDelivery.Shared.Contracts.Domain.Models;
using Restaurant.Application.DTOs.MenuManagement;
using Restaurant.Application.Services;

namespace PureDelivery.RestaurantService.Controllers
{
    /// <summary>
    /// Управление блюдами, опциями и чойсами — только для менеджеров ресторана.
    /// </summary>
    [ApiController]
    [Route("api/v1/restaurant/[controller]")]
    [Produces("application/json")]
    public class MenuManagementController : ControllerBase
    {
        private readonly IMenuManagementService _menuManagementService;
        private readonly ILogger<MenuManagementController> _logger;

        public MenuManagementController(
            IMenuManagementService menuManagementService,
            ILogger<MenuManagementController> logger)
        {
            _menuManagementService = menuManagementService;
            _logger = logger;
        }

        // ── MenuItem ──────────────────────────────────────────────────────────

        [HttpGet("{restaurantId:guid}/items")]
        [ProducesResponseType(typeof(BaseResponse<List<MenuItemManagementDto>>), StatusCodes.Status200OK)]
        public async Task<ActionResult<BaseResponse<List<MenuItemManagementDto>>>> GetAllItems(
            [FromRoute] Guid restaurantId, CancellationToken ct = default)
        {
            var result = await _menuManagementService.GetAllItemsAsync(restaurantId, ct);
            if (!result.IsSuccess) return BadRequest(result);
            return Ok(result);
        }

        [HttpGet("{restaurantId:guid}/items/{itemId:guid}")]
        [ProducesResponseType(typeof(BaseResponse<MenuItemManagementDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<MenuItemManagementDto>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<BaseResponse<MenuItemManagementDto>>> GetItem(
            [FromRoute] Guid restaurantId, [FromRoute] Guid itemId, CancellationToken ct = default)
        {
            var result = await _menuManagementService.GetItemAsync(restaurantId, itemId, ct);
            if (!result.IsSuccess) return NotFound(result);
            return Ok(result);
        }

        [HttpPost("{restaurantId:guid}/items")]
        [ProducesResponseType(typeof(BaseResponse<MenuItemManagementDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(BaseResponse<MenuItemManagementDto>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<BaseResponse<MenuItemManagementDto>>> CreateItem(
            [FromRoute] Guid restaurantId,
            [FromBody] CreateMenuItemRequest request,
            CancellationToken ct = default)
        {
            if (!ModelState.IsValid)
                return BadRequest(BaseResponse<MenuItemManagementDto>.Failure("Invalid request data"));

            var result = await _menuManagementService.CreateItemAsync(restaurantId, request, ct);
            if (!result.IsSuccess) return BadRequest(result);

            return CreatedAtAction(nameof(GetItem),
                new { restaurantId, itemId = result.Data!.Id }, result);
        }

        [HttpPut("{restaurantId:guid}/items/{itemId:guid}")]
        [ProducesResponseType(typeof(BaseResponse<MenuItemManagementDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<MenuItemManagementDto>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<BaseResponse<MenuItemManagementDto>>> UpdateItem(
            [FromRoute] Guid restaurantId,
            [FromRoute] Guid itemId,
            [FromBody] UpdateMenuItemRequest request,
            CancellationToken ct = default)
        {
            var result = await _menuManagementService.UpdateItemAsync(restaurantId, itemId, request, ct);
            if (!result.IsSuccess) return NotFound(result);
            return Ok(result);
        }

        [HttpDelete("{restaurantId:guid}/items/{itemId:guid}")]
        [ProducesResponseType(typeof(BaseResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<bool>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<BaseResponse<bool>>> DeleteItem(
            [FromRoute] Guid restaurantId, [FromRoute] Guid itemId, CancellationToken ct = default)
        {
            var result = await _menuManagementService.DeleteItemAsync(restaurantId, itemId, ct);
            if (!result.IsSuccess) return NotFound(result);
            return Ok(result);
        }

        // ── Option ────────────────────────────────────────────────────────────

        [HttpPost("{restaurantId:guid}/items/{itemId:guid}/options")]
        [ProducesResponseType(typeof(BaseResponse<OptionManagementDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(BaseResponse<OptionManagementDto>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<BaseResponse<OptionManagementDto>>> AddOption(
            [FromRoute] Guid restaurantId,
            [FromRoute] Guid itemId,
            [FromBody] CreateMenuItemOptionRequest request,
            CancellationToken ct = default)
        {
            if (!ModelState.IsValid)
                return BadRequest(BaseResponse<OptionManagementDto>.Failure("Invalid request data"));

            var result = await _menuManagementService.AddOptionAsync(restaurantId, itemId, request, ct);
            if (!result.IsSuccess) return NotFound(result);
            return StatusCode(StatusCodes.Status201Created, result);
        }

        [HttpPut("{restaurantId:guid}/items/{itemId:guid}/options/{optionId:guid}")]
        [ProducesResponseType(typeof(BaseResponse<OptionManagementDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<OptionManagementDto>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<BaseResponse<OptionManagementDto>>> UpdateOption(
            [FromRoute] Guid restaurantId,
            [FromRoute] Guid itemId,
            [FromRoute] Guid optionId,
            [FromBody] UpdateMenuItemOptionRequest request,
            CancellationToken ct = default)
        {
            var result = await _menuManagementService.UpdateOptionAsync(restaurantId, itemId, optionId, request, ct);
            if (!result.IsSuccess) return NotFound(result);
            return Ok(result);
        }

        [HttpDelete("{restaurantId:guid}/items/{itemId:guid}/options/{optionId:guid}")]
        [ProducesResponseType(typeof(BaseResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<bool>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<BaseResponse<bool>>> DeleteOption(
            [FromRoute] Guid restaurantId,
            [FromRoute] Guid itemId,
            [FromRoute] Guid optionId,
            CancellationToken ct = default)
        {
            var result = await _menuManagementService.DeleteOptionAsync(restaurantId, itemId, optionId, ct);
            if (!result.IsSuccess) return NotFound(result);
            return Ok(result);
        }

        // ── Choice ────────────────────────────────────────────────────────────

        [HttpPost("{restaurantId:guid}/items/{itemId:guid}/options/{optionId:guid}/choices")]
        [ProducesResponseType(typeof(BaseResponse<ChoiceManagementDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(BaseResponse<ChoiceManagementDto>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<BaseResponse<ChoiceManagementDto>>> AddChoice(
            [FromRoute] Guid restaurantId,
            [FromRoute] Guid itemId,
            [FromRoute] Guid optionId,
            [FromBody] CreateOptionChoiceRequest request,
            CancellationToken ct = default)
        {
            if (!ModelState.IsValid)
                return BadRequest(BaseResponse<ChoiceManagementDto>.Failure("Invalid request data"));

            var result = await _menuManagementService.AddChoiceAsync(restaurantId, itemId, optionId, request, ct);
            if (!result.IsSuccess) return NotFound(result);
            return StatusCode(StatusCodes.Status201Created, result);
        }

        [HttpPut("{restaurantId:guid}/items/{itemId:guid}/options/{optionId:guid}/choices/{choiceId:guid}")]
        [ProducesResponseType(typeof(BaseResponse<ChoiceManagementDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<ChoiceManagementDto>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<BaseResponse<ChoiceManagementDto>>> UpdateChoice(
            [FromRoute] Guid restaurantId,
            [FromRoute] Guid itemId,
            [FromRoute] Guid optionId,
            [FromRoute] Guid choiceId,
            [FromBody] UpdateOptionChoiceRequest request,
            CancellationToken ct = default)
        {
            var result = await _menuManagementService.UpdateChoiceAsync(restaurantId, itemId, optionId, choiceId, request, ct);
            if (!result.IsSuccess) return NotFound(result);
            return Ok(result);
        }

        [HttpDelete("{restaurantId:guid}/items/{itemId:guid}/options/{optionId:guid}/choices/{choiceId:guid}")]
        [ProducesResponseType(typeof(BaseResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<bool>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<BaseResponse<bool>>> DeleteChoice(
            [FromRoute] Guid restaurantId,
            [FromRoute] Guid itemId,
            [FromRoute] Guid optionId,
            [FromRoute] Guid choiceId,
            CancellationToken ct = default)
        {
            var result = await _menuManagementService.DeleteChoiceAsync(restaurantId, itemId, optionId, choiceId, ct);
            if (!result.IsSuccess) return NotFound(result);
            return Ok(result);
        }
    }
}
