using Microsoft.EntityFrameworkCore;
using Restaurant.Application.Repositories;
using RestaurantService.Domain.Entities;
using RestaurantService.Infrastructure.Data;

namespace Restaurant.Infrastructure.Repositories;

public class RestaurantReviewRepository(RestaurantDbContext context) : IRestaurantReviewRepository
{
    public async Task<bool> HasReviewedAsync(Guid restaurantId, Guid orderId, Guid customerId, CancellationToken ct = default) =>
        await context.RestaurantReviews.AnyAsync(
            r => r.RestaurantId == restaurantId && r.OrderId == orderId && r.CustomerId == customerId, ct);

    public async Task<RestaurantReview> AddAsync(RestaurantReview review, CancellationToken ct = default)
    {
        context.RestaurantReviews.Add(review);
        await context.SaveChangesAsync(ct);
        return review;
    }

    public async Task<List<RestaurantReview>> GetByRestaurantIdAsync(
        Guid restaurantId, int page, int pageSize, CancellationToken ct = default) =>
        await context.RestaurantReviews
            .Where(r => r.RestaurantId == restaurantId)
            .OrderByDescending(r => r.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

    public async Task<int> GetTotalCountAsync(Guid restaurantId, CancellationToken ct = default) =>
        await context.RestaurantReviews.CountAsync(r => r.RestaurantId == restaurantId, ct);

    public async Task<double> GetAverageRatingAsync(Guid restaurantId, CancellationToken ct = default)
    {
        var ratings = await context.RestaurantReviews
            .Where(r => r.RestaurantId == restaurantId)
            .Select(r => r.Rating)
            .ToListAsync(ct);
        return ratings.Count == 0 ? 0 : ratings.Average();
    }
}
