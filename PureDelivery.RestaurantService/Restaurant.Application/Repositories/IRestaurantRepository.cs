using PureDelivery.Shared.Contracts.Domain.Models;
using PureDelivery.Shared.Contracts.DTOs.Restaurants.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestaurantDomain = RestaurantService.Domain.Entities.Restaurant;

namespace Restaurant.Application.Repositories
{
    public interface IRestaurantRepository
    {
        Task<List<RestaurantDomain>> GetRestaurantsDataAsync();
        Task<RestaurantDomain?> GetRestaurantByIdAsync(Guid restaurantId);
        Task<List<RestaurantDomain>> SearchRestaurantsByNameAsync(string query);
        Task<List<RestaurantDomain>> SearchRestaurantsByDishNameAsync(string dishQuery);
        Task<List<RestaurantDomain>> FilterRestaurantsAsync(List<string>? cuisineTypes, List<string>? tags);
    }
}
