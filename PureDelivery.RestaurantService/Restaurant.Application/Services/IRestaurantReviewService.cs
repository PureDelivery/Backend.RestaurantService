using PureDelivery.Shared.Contracts.Domain.Models;
using Restaurant.Application.DTOs.Reviews;

namespace Restaurant.Application.Services;

public interface IRestaurantReviewService
{
    Task<BaseResponse<RestaurantReviewDto>> SubmitReviewAsync(Guid restaurantId, SubmitReviewRequest request, CancellationToken ct = default);
    Task<BaseResponse<PagedReviewsDto>> GetReviewsAsync(Guid restaurantId, int page, int pageSize, CancellationToken ct = default);
}
