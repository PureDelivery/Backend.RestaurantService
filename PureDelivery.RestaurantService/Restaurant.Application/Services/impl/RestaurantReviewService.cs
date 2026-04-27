using MassTransit;
using Microsoft.Extensions.Logging;
using PureDelivery.Shared.Contracts.Domain.Models;
using PureDelivery.Shared.Contracts.Events.Reviews;
using Restaurant.Application.DTOs.Reviews;
using Restaurant.Application.Repositories;
using RestaurantService.Domain.Entities;

namespace Restaurant.Application.Services.impl;

public class RestaurantReviewService(
    IRestaurantReviewRepository reviewRepo,
    IRestaurantRepository restaurantRepo,
    IPublishEndpoint publishEndpoint,
    ILogger<RestaurantReviewService> logger) : IRestaurantReviewService
{
    public async Task<BaseResponse<RestaurantReviewDto>> SubmitReviewAsync(
        Guid restaurantId, SubmitReviewRequest request, CancellationToken ct = default)
    {
        try
        {
            var restaurant = await restaurantRepo.GetRestaurantByIdAsync(restaurantId);
            if (restaurant == null)
                return BaseResponse<RestaurantReviewDto>.Failure("Restaurant not found.");

            if (await reviewRepo.HasReviewedAsync(restaurantId, request.OrderId, request.CustomerId, ct))
                return BaseResponse<RestaurantReviewDto>.Failure("You have already reviewed this restaurant for this order.");

            var review = new RestaurantReview
            {
                Id           = Guid.NewGuid(),
                RestaurantId = restaurantId,
                CustomerId   = request.CustomerId,
                OrderId      = request.OrderId,
                Rating       = request.Rating,
                Comment      = request.Comment?.Trim(),
                CreatedAt    = DateTime.UtcNow,
            };

            await reviewRepo.AddAsync(review, ct);

            logger.LogInformation("Review {Rating} submitted for restaurant {RestaurantId} on order {OrderId}",
                request.Rating, restaurantId, request.OrderId);

            await publishEndpoint.Publish(new RestaurantReviewSubmittedEvent
            {
                ReviewId     = review.Id,
                RestaurantId = restaurantId,
                CustomerId   = request.CustomerId,
                CustomerName = request.CustomerName ?? string.Empty,
                OrderId      = request.OrderId,
                Rating       = request.Rating,
                Comment      = request.Comment,
                CreatedAt    = review.CreatedAt,
            }, ct);

            return BaseResponse<RestaurantReviewDto>.Success(review.ToDto());
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error submitting review for restaurant {RestaurantId}", restaurantId);
            return BaseResponse<RestaurantReviewDto>.Failure($"Error: {ex.Message}");
        }
    }

    public async Task<BaseResponse<PagedReviewsDto>> GetReviewsAsync(
        Guid restaurantId, int page, int pageSize, CancellationToken ct = default)
    {
        try
        {
            var restaurant = await restaurantRepo.GetRestaurantByIdAsync(restaurantId);
            if (restaurant == null)
                return BaseResponse<PagedReviewsDto>.Failure("Restaurant not found.");

            var reviews   = await reviewRepo.GetByRestaurantIdAsync(restaurantId, page, pageSize, ct);
            var total     = await reviewRepo.GetTotalCountAsync(restaurantId, ct);
            var avgRating = await reviewRepo.GetAverageRatingAsync(restaurantId, ct);

            return BaseResponse<PagedReviewsDto>.Success(new PagedReviewsDto
            {
                Items         = reviews.Select(r => r.ToDto()).ToList(),
                TotalCount    = total,
                Page          = page,
                PageSize      = pageSize,
                AverageRating = Math.Round(avgRating, 2),
            });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting reviews for restaurant {RestaurantId}", restaurantId);
            return BaseResponse<PagedReviewsDto>.Failure($"Error: {ex.Message}");
        }
    }
}

internal static class RestaurantReviewExtensions
{
    public static RestaurantReviewDto ToDto(this RestaurantReview r) => new()
    {
        Id           = r.Id,
        RestaurantId = r.RestaurantId,
        CustomerId   = r.CustomerId,
        OrderId      = r.OrderId,
        Rating       = r.Rating,
        Comment      = r.Comment,
        CreatedAt    = r.CreatedAt,
    };
}
