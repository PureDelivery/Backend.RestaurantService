using Microsoft.EntityFrameworkCore;
using PureDelivery.Shared.Contracts.DTOs.Restaurants.Requests;
using Restaurant.Application.Repositories;
using RestaurantService.Domain.Enums;
using RestaurantService.Infrastructure.Data;
using RestaurantDomain = RestaurantService.Domain.Entities.Restaurant;

namespace Restaurant.Infrastructure.Repositories
{
    public class RestaurantRepository : IRestaurantRepository
    {
        private readonly RestaurantDbContext _context;

        public RestaurantRepository(RestaurantDbContext context)
        {
            _context = context;
        }

        public async Task<RestaurantDomain?> GetRestaurantByIdAsync(Guid restaurantId)
        {
            var restaurant = await _context.Restaurants
            .FirstOrDefaultAsync(r => r.Id == restaurantId && r.IsActive);

            if (restaurant == null)
                return null;

            await _context.Entry(restaurant)
            .Reference(r => r.Address)
            .LoadAsync();

            await _context.Entry(restaurant)
                .Reference(r => r.Settings)
                .LoadAsync();

            await _context.Entry(restaurant)
                .Reference(r => r.Partnership)
                .LoadAsync();

            await _context.Entry(restaurant)
                .Reference(r => r.Marketing)
                .LoadAsync();

            await _context.Entry(restaurant)
                .Collection(r => r.Reviews)
                .LoadAsync();

            await _context.Entry(restaurant)
                .Collection(r => r.WorkingHours)
                .LoadAsync();

            await _context.Entry(restaurant)
                    .Collection(r => r.DeliveryZones)
                    .LoadAsync();

            foreach (var zone in restaurant.DeliveryZones)
            {
                await _context.Entry(zone)
                    .Collection(z => z.Points)
                    .LoadAsync();
            }

            return restaurant;
        }

        public async Task<List<RestaurantDomain>> GetRestaurantsDataAsync()
        {
            return await _context.Restaurants
                        .Include(r => r.Address)
                        .Include(r => r.Settings)
                        .Include(r => r.Partnership)
                        .Include(r => r.Marketing)
                        .Include(r => r.Reviews)
                        .Include(r => r.WorkingHours)
                        .Include(r => r.DeliveryZones)
                            .ThenInclude(r => r.Points)
                        .Where(r => r.IsActive)
                        .ToListAsync();
        }

        public async Task<List<RestaurantDomain>> SearchRestaurantsByNameAsync(string query)
        {
            return await _context.Restaurants
                .Include(r => r.Address)
                .Include(r => r.Settings)
                .Include(r => r.Partnership)
                .Include(r => r.Marketing)
                .Include(r => r.Reviews)
                .Include(r => r.WorkingHours)
                .Include(r => r.DeliveryZones)
                    .ThenInclude(r => r.Points)
                .Where(r => r.IsActive &&
                           (r.Name.Contains(query) ||
                            r.Description.Contains(query)))
                .ToListAsync();
        }

        public async Task<List<RestaurantDomain>> SearchRestaurantsByDishNameAsync(string dishQuery)
        {
            return await _context.Restaurants
                .Include(r => r.Address)
                .Include(r => r.Settings)
                .Include(r => r.Partnership)
                .Include(r => r.Marketing)
                .Include(r => r.Reviews)
                .Include(r => r.WorkingHours)
                .Include(r => r.DeliveryZones)
                     .ThenInclude(r => r.Points)
                .Include(r => r.MenuItems)
                .Where(r => r.IsActive &&
                           r.MenuItems.Any(mi => mi.IsAvailable &&
                                               (mi.Name.Contains(dishQuery) ||
                                                mi.Description.Contains(dishQuery))))
                .ToListAsync();
        }

        public async Task<List<RestaurantDomain>> FilterRestaurantsAsync(
            List<string>? cuisineTypes,
            List<string>? tags)
        {
            // Базовый запрос всех активных ресторанов
            var query = _context.Restaurants
                .Include(r => r.Address)
                .Include(r => r.Settings)
                .Include(r => r.Partnership)
                .Include(r => r.Marketing)
                .Include(r => r.Reviews)
                .Include(r => r.WorkingHours)
                .Include(r => r.DeliveryZones)
                     .ThenInclude(r => r.Points)
                .Where(r => r.IsActive);

            // Загружаем все рестораны в память
            var restaurants = await query.ToListAsync();

            // Применяем фильтры последовательно
            var filteredRestaurants = restaurants.AsEnumerable();

            // Фильтр по кухням (если указаны)
            if (cuisineTypes != null && cuisineTypes.Any())
            {
                var cuisineEnums = cuisineTypes
                    .Select(cuisine => Enum.TryParse<CuisineType>(cuisine, out var result) ? result : (CuisineType?)null)
                    .Where(cuisine => cuisine != null)
                    .ToList();

                if (cuisineEnums.Any())
                {
                    filteredRestaurants = filteredRestaurants.Where(r =>
                        cuisineEnums.Any(cuisine =>
                            Enum.TryParse<CuisineType>(r.CuisineTypes.ToString(), out var restaurantCuisines) &&
                            restaurantCuisines.HasFlag(cuisine.Value)));
                }
            }

            // Фильтр по тегам (если указаны)
            if (tags != null && tags.Any())
            {
                var tagEnums = tags
                    .Select(tag => Enum.TryParse<RestaurantTag>(tag, out var result) ? result : (RestaurantTag?)null)
                    .Where(tag => tag != null)
                    .ToList();

                if (tagEnums.Any())
                {
                    filteredRestaurants = filteredRestaurants.Where(r =>
                        tagEnums.Any(tag =>
                            Enum.TryParse<RestaurantTag>(r.Tags.ToString(), out var restaurantTags) &&
                            restaurantTags.HasFlag(tag.Value)));
                }
            }

            return filteredRestaurants.ToList();
        }        
    }
}
