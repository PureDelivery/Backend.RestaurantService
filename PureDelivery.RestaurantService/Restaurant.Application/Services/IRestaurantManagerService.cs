using PureDelivery.Shared.Contracts.Domain.Models;
using Restaurant.Application.DTOs;

namespace Restaurant.Application.Services
{
    public interface IRestaurantManagerService
    {
        Task<BaseResponse<ManagerDto>> CreateManagerAsync(CreateManagerRequest request, CancellationToken ct = default);
        Task<BaseResponse<ManagerDto>> GetByIdAsync(Guid id, CancellationToken ct = default);
        Task<BaseResponse<ManagerDto>> GetByUserIdAsync(Guid userId, CancellationToken ct = default);
        Task<BaseResponse<List<ManagerDto>>> GetByRestaurantIdAsync(Guid restaurantId, CancellationToken ct = default);
        Task<BaseResponse<bool>> DeactivateAsync(Guid id, CancellationToken ct = default);
    }
}
