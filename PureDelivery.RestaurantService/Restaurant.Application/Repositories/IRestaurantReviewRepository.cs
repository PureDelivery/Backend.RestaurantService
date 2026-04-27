using RestaurantService.Domain.Entities;

namespace Restaurant.Application.Repositories;

public interface IRestaurantReviewRepository
{
    Task<bool> HasReviewedAsync(Guid restaurantId, Guid orderId, Guid customerId, CancellationToken ct = default);
    Task<RestaurantReview> AddAsync(RestaurantReview review, CancellationToken ct = default);
    Task<List<RestaurantReview>> GetByRestaurantIdAsync(Guid restaurantId, int page, int pageSize, CancellationToken ct = default);
    Task<int> GetTotalCountAsync(Guid restaurantId, CancellationToken ct = default);
    Task<double> GetAverageRatingAsync(Guid restaurantId, CancellationToken ct = default);
}
