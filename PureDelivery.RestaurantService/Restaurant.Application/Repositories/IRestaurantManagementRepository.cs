using RestaurantService.Domain.Entities;
using RestaurantDomain = RestaurantService.Domain.Entities.Restaurant;

namespace Restaurant.Application.Repositories
{
    public interface IRestaurantManagementRepository
    {
        Task<RestaurantDomain?> GetForManagementAsync(Guid restaurantId, CancellationToken ct = default);
        Task UpdateRestaurantAsync(RestaurantDomain restaurant, CancellationToken ct = default);
        Task UpdateSettingsAsync(RestaurantSettings settings, CancellationToken ct = default);
        Task<RestaurantMarketing?> GetMarketingAsync(Guid restaurantId, CancellationToken ct = default);
        Task UpsertMarketingAsync(RestaurantMarketing marketing, CancellationToken ct = default);
        Task<List<WorkingHours>> GetWorkingHoursAsync(Guid restaurantId, CancellationToken ct = default);
        Task UpsertWorkingHoursAsync(Guid restaurantId, List<WorkingHours> hours, CancellationToken ct = default);
        Task<List<RestaurantMarketing>> GetAllFeaturedOrderByPriorityAsync(CancellationToken ct);
    }
}
