using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using PureDelivery.Shared.Contracts.Common;
using PureDelivery.Shared.Contracts.Domain.Models;
using PureDelivery.Shared.Contracts.DTOs.Restaurants.Requests;
using PureDelivery.Shared.Contracts.DTOs.Restaurants.Responses;
using Restaurant.Application.Services;

namespace PureDelivery.RestaurantService.Controllers;

[ApiController]
[Route("api/v1/restaurant/[controller]")]
public class MenuController : ControllerBase
{
    private readonly ILogger<MenuController> _logger;
    private readonly IMenuService _menuService;

    public MenuController(
        ILogger<MenuController> logger,
        IMenuService menuService)
    {
        _logger = logger;
        _menuService = menuService;
    }

    /// <summary>
    /// Получение меню ресторана
    /// </summary>
    [HttpGet("{restaurantId}")]
    public async Task<ActionResult<BaseResponse<RestaurantMenuDto>>> GetRestaurantMenu(Guid restaurantId)
    {
        try
        {
            _logger.LogInformation("Getting menu for restaurant: {RestaurantId}", restaurantId);

            var menu = await _menuService.GetRestaurantMenuAsync(restaurantId);

            if (menu == null)
            {
                return Ok(new BaseResponse<RestaurantMenuDto>
                {
                    IsSuccess = false,
                    Error = "Restaurant menu not found"
                });
            }

            return Ok(new BaseResponse<RestaurantMenuDto>
            {
                IsSuccess = true,
                Data = menu
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting menu for restaurant {RestaurantId}", restaurantId);
            return Ok(new BaseResponse<RestaurantMenuDto>
            {
                IsSuccess = false,
                Error = "Failed to retrieve restaurant menu"
            });
        }
    }

    /// <summary>
    /// Получение блюда более детально
    /// </summary>
    [HttpGet("{restaurantId}/item/{menuItemId}")]
    public async Task<ActionResult<BaseResponse<MenuItemDetailDto>>> GetMenuItemDetail(
        Guid restaurantId, Guid menuItemId)
    {
        try
        {
            _logger.LogInformation("Getting menu item detail: {MenuItemId} from restaurant: {RestaurantId}",
                menuItemId, restaurantId);

            var menuItem = await _menuService.GetMenuItemDetailAsync(restaurantId, menuItemId);

            if (menuItem == null)
            {
                return Ok(new BaseResponse<MenuItemDetailDto>
                {
                    IsSuccess = false,
                    Error = "Menu item not found"
                });
            }

            return Ok(new BaseResponse<MenuItemDetailDto>
            {
                IsSuccess = true,
                Data = menuItem
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting menu item detail {MenuItemId} from restaurant {RestaurantId}",
                menuItemId, restaurantId);
            return Ok(new BaseResponse<MenuItemDetailDto>
            {
                IsSuccess = false,
                Error = "Failed to retrieve menu item details"
            });
        }
    }

    /// <summary>
    /// Поиск по блюду в ресторане
    /// </summary>
    [HttpPost("{restaurantId}/search")]
    public async Task<ActionResult<BaseResponse<PagedResult<MenuItemDto>>>> SearchDishesInRestaurant(
        Guid restaurantId, [FromBody] SearchDishInRestaurantRequest request)
    {
        try
        {
            _logger.LogInformation("Searching dishes in restaurant {RestaurantId} for: {Query}",
                restaurantId, request.Query);

            var results = await _menuService.SearchDishesInRestaurantAsync(restaurantId, request);

            return Ok(new BaseResponse<PagedResult<MenuItemDto>>
            {
                IsSuccess = true,
                Data = results
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching dishes in restaurant {RestaurantId} for: {Query}",
                restaurantId, request.Query);
            return Ok(new BaseResponse<PagedResult<MenuItemDto>>
            {
                IsSuccess = false,
                Error = "Dish search in restaurant failed"
            });
        }
    }

    /// <summary>
    /// Фильтр по MenuCategory
    /// </summary>
    [HttpPost("{restaurantId}/filter/category")]
    public async Task<ActionResult<BaseResponse<PagedResult<MenuItemDto>>>> FilterByMenuCategory(
        Guid restaurantId, [FromBody] FilterByMenuCategoryRequest request)
    {
        try
        {
            _logger.LogInformation("Filtering menu items in restaurant {RestaurantId} by categories: {Categories}",
                restaurantId, string.Join(", ", request.Categories));

            var results = await _menuService.FilterByMenuCategoryAsync(restaurantId, request);

            return Ok(new BaseResponse<PagedResult<MenuItemDto>>
            {
                IsSuccess = true,
                Data = results
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error filtering by menu categories in restaurant {RestaurantId}: {Categories}",
                restaurantId, string.Join(", ", request.Categories));
            return Ok(new BaseResponse<PagedResult<MenuItemDto>>
            {
                IsSuccess = false,
                Error = "Menu category filter failed"
            });
        }
    }

    /// <summary>
    /// Получение информации по списку ID блюд (Bulk) для OrderService
    /// </summary>
    [HttpPost("items/bulk")]
    public async Task<ActionResult<BaseResponse<List<MenuItemDetailDto>>>> GetMenuItemsBulk([FromBody] List<Guid> itemIds)
    {
        try
        {
            _logger.LogInformation("Bulk fetching {Count} menu items", itemIds.Count);

            // Метод в IMenuService, который делает: _db.Items.Where(i => itemIds.Contains(i.Id)).ToList()
            var items = await _menuService.GetMenuItemsByIdsAsync(itemIds);

            return Ok(new BaseResponse<List<MenuItemDetailDto>>
            {
                IsSuccess = true,
                Data = items
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching bulk items");
            return StatusCode(500, new BaseResponse<List<MenuItemDetailDto>> { IsSuccess = false, Error = "Bulk fetch failed" });
        }
    }
}