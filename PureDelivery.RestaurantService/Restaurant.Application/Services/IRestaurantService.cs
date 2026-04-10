using PureDelivery.Shared.Contracts.Domain.Models;
using PureDelivery.Shared.Contracts.DTOs.Restaurants.Requests;
using PureDelivery.Shared.Contracts.DTOs.Restaurants.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant.Application.Services
{
    public interface IRestaurantService
    {
        Task<PagedResult<RestaurantDto>> GetRestaurantsByLocationAsync(GetRestaurantsRequest request);
        Task<RestaurantDetailDto> GetRestaurantDetailAsync(Guid restaurantId, GetRestaurantsRequest request);
        Task<PagedResult<RestaurantDto>> SearchRestaurantsAsync(SearchRestaurantRequest request);
        Task<PagedResult<RestaurantDto>> SearchRestaurantsByDishAsync(SearchRestaurantByDishRequest request);
        Task<PagedResult<RestaurantDto>> FilterRestaurantsAsync(FilterRestaurantsRequest request);
        Task<RestaurantDetailDto> GetRestaurantByIdAsync(Guid restaurantId);
    }
}
