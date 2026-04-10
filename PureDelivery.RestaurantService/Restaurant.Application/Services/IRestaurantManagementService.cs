using PureDelivery.Shared.Contracts.Domain.Models;
using Restaurant.Application.DTOs.RestaurantManagement;

namespace Restaurant.Application.Services
{
    public interface IRestaurantManagementService
    {
        Task<BaseResponse<RestaurantManagementDetailDto>> GetRestaurantAsync(Guid restaurantId, CancellationToken ct = default);
        Task<BaseResponse<RestaurantManagementDetailDto>> UpdateInfoAsync(Guid restaurantId, UpdateRestaurantInfoRequest request, CancellationToken ct = default);
        Task<BaseResponse<RestaurantSettingsManagementDto>> UpdateSettingsAsync(Guid restaurantId, UpdateRestaurantSettingsRequest request, CancellationToken ct = default);
        Task<BaseResponse<List<WorkingHoursManagementDto>>> UpsertWorkingHoursAsync(Guid restaurantId, UpsertWorkingHoursRequest request, CancellationToken ct = default);
    }
}
