using PureDelivery.Shared.Contracts.Domain.Models;
using PureDelivery.Shared.Contracts.DTOs.Restaurants.Requests;
using PureDelivery.Shared.Contracts.DTOs.Restaurants.Responses;

namespace Restaurant.Application.Services
{
    public interface IMenuService
    {
        Task<RestaurantMenuDto?> GetRestaurantMenuAsync(Guid restaurantId);
        Task<MenuItemDetailDto?> GetMenuItemDetailAsync(Guid restaurantId, Guid menuItemId);
        Task<PagedResult<MenuItemDto>> SearchDishesInRestaurantAsync(Guid restaurantId, SearchDishInRestaurantRequest request);
        Task<PagedResult<MenuItemDto>> FilterByMenuCategoryAsync(Guid restaurantId, FilterByMenuCategoryRequest request);
        Task<List<MenuItemDetailDto>> GetMenuItemsByIdsAsync(List<Guid> itemIds);
    }
}
