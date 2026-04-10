using PureDelivery.Shared.Contracts.DTOs.Restaurants.Responses;
using RestaurantService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant.Application.Mappers
{
    public interface IMenuMapper
    {
        RestaurantMenuDto MapToMenuDto(Guid restaurantId, string restaurantName, List<MenuItem> menuItems);
        MenuItemDetailDto MapToDetailDto(MenuItem menuItem);
        List<MenuItemDto> MapToDtos(IEnumerable<MenuItem> menuItems);
    }
}
