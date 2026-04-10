using PureDelivery.Shared.Contracts.Domain.Models;
using PureDelivery.Shared.Contracts.DTOs.Restaurants.Requests;
using PureDelivery.Shared.Contracts.DTOs.Restaurants.Responses;
using Restaurant.Application.Mappers;
using Restaurant.Application.Pagination;
using Restaurant.Application.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant.Application.Services.impl
{
    public class MenuService : IMenuService
    {
        private readonly IMenuRepository _menuRepository;
        private readonly IRestaurantRepository _restaurantRepository;
        private readonly IMenuMapper _menuMapper;
        private readonly IPaginationService _paginationService;

        public MenuService(
            IMenuRepository menuRepository,
            IRestaurantRepository restaurantRepository,
            IMenuMapper menuMapper,
            IPaginationService paginationService)
        {
            _menuRepository = menuRepository;
            _restaurantRepository = restaurantRepository;
            _menuMapper = menuMapper;
            _paginationService = paginationService;
        }



        // TODO: ADD PATTERN LIKE TEMPLATE METHOD



        public async Task<RestaurantMenuDto?> GetRestaurantMenuAsync(Guid restaurantId)
        {
            var restaurant = await _restaurantRepository.GetRestaurantByIdAsync(restaurantId);
            if (restaurant == null)
                return null;

            var menuItems = await _menuRepository.GetMenuItemsByRestaurantIdAsync(restaurantId);

            return _menuMapper.MapToMenuDto(restaurantId, restaurant.Name, menuItems);
        }

        public async Task<MenuItemDetailDto?> GetMenuItemDetailAsync(Guid restaurantId, Guid menuItemId)
        {
            var menuItem = await _menuRepository.GetMenuItemByIdAsync(restaurantId, menuItemId);

            if (menuItem == null)
                return null;

            return _menuMapper.MapToDetailDto(menuItem);
        }

        public async Task<PagedResult<MenuItemDto>> SearchDishesInRestaurantAsync(
            Guid restaurantId,
            SearchDishInRestaurantRequest request)
        {
            var foundMenuItems = await _menuRepository.SearchMenuItemsInRestaurantAsync(restaurantId, request.Query);

            var menuItemDtos = _menuMapper.MapToDtos(foundMenuItems);

            return _paginationService.CreatePagedResult(menuItemDtos, request.Page, request.PageSize);
        }

        public async Task<PagedResult<MenuItemDto>> FilterByMenuCategoryAsync(
            Guid restaurantId,
            FilterByMenuCategoryRequest request)
        {
            var filteredMenuItems = await _menuRepository.FilterMenuItemsByCategoryAsync(restaurantId, request.Categories);

            var menuItemDtos = _menuMapper.MapToDtos(filteredMenuItems);

            return _paginationService.CreatePagedResult(menuItemDtos, request.Page, request.PageSize);
        }

        public async Task<List<MenuItemDetailDto>> GetMenuItemsByIdsAsync(List<Guid> itemIds)
        {
            var menuItems = await _menuRepository.GetMenuItemsByIdsAsync(itemIds);

            var menuItemDetailDtos = menuItems.Select(itemIds => _menuMapper.MapToDetailDto(itemIds)).ToList();

            return menuItemDetailDtos;
        }
    }
}
