using RestaurantService.Domain.Entities;

namespace Restaurant.Application.Repositories
{
    public interface IRestaurantManagerRepository
    {
        Task<RestaurantManager?> GetByIdAsync(Guid id, CancellationToken ct = default);
        Task<RestaurantManager?> GetByUserIdAsync(Guid userId, CancellationToken ct = default);
        Task<List<RestaurantManager>> GetByRestaurantIdAsync(Guid restaurantId, CancellationToken ct = default);
        Task<RestaurantManager> AddAsync(RestaurantManager manager, CancellationToken ct = default);
        Task<bool> DeactivateAsync(Guid id, CancellationToken ct = default);
    }
}
