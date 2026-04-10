using PureDelivery.Shared.Contracts.Domain.Models;
using Restaurant.Application.DTOs.AdminMarketing;

namespace Restaurant.Application.Services
{
    public interface IAdminMarketingService
    {
        Task<BaseResponse<AdminMarketingDto>> GetMarketingAsync(Guid restaurantId, CancellationToken ct = default);
        Task<BaseResponse<AdminMarketingDto>> UpdateMarketingAsync(Guid restaurantId, UpdateRestaurantMarketingRequest request, CancellationToken ct = default);
        Task<BaseResponse<List<AdminMarketingDto>>> GetAllFeaturedAsync(CancellationToken ct = default);
    }
}
