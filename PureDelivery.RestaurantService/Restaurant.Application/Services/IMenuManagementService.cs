using PureDelivery.Shared.Contracts.Domain.Models;
using Restaurant.Application.DTOs.MenuManagement;

namespace Restaurant.Application.Services
{
    public interface IMenuManagementService
    {
        // MenuItem
        Task<BaseResponse<List<MenuItemManagementDto>>> GetAllItemsAsync(Guid restaurantId, CancellationToken ct = default);
        Task<BaseResponse<MenuItemManagementDto>> GetItemAsync(Guid restaurantId, Guid itemId, CancellationToken ct = default);
        Task<BaseResponse<MenuItemManagementDto>> CreateItemAsync(Guid restaurantId, CreateMenuItemRequest request, CancellationToken ct = default);
        Task<BaseResponse<MenuItemManagementDto>> UpdateItemAsync(Guid restaurantId, Guid itemId, UpdateMenuItemRequest request, CancellationToken ct = default);
        Task<BaseResponse<bool>> DeleteItemAsync(Guid restaurantId, Guid itemId, CancellationToken ct = default);

        // Option
        Task<BaseResponse<OptionManagementDto>> AddOptionAsync(Guid restaurantId, Guid itemId, CreateMenuItemOptionRequest request, CancellationToken ct = default);
        Task<BaseResponse<OptionManagementDto>> UpdateOptionAsync(Guid restaurantId, Guid itemId, Guid optionId, UpdateMenuItemOptionRequest request, CancellationToken ct = default);
        Task<BaseResponse<bool>> DeleteOptionAsync(Guid restaurantId, Guid itemId, Guid optionId, CancellationToken ct = default);

        // Choice
        Task<BaseResponse<ChoiceManagementDto>> AddChoiceAsync(Guid restaurantId, Guid itemId, Guid optionId, CreateOptionChoiceRequest request, CancellationToken ct = default);
        Task<BaseResponse<ChoiceManagementDto>> UpdateChoiceAsync(Guid restaurantId, Guid itemId, Guid optionId, Guid choiceId, UpdateOptionChoiceRequest request, CancellationToken ct = default);
        Task<BaseResponse<bool>> DeleteChoiceAsync(Guid restaurantId, Guid itemId, Guid optionId, Guid choiceId, CancellationToken ct = default);
    }
}
