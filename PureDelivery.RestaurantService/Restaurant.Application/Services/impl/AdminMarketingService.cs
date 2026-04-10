using Microsoft.Extensions.Logging;
using PureDelivery.Shared.Contracts.Domain.Models;
using Restaurant.Application.DTOs.AdminMarketing;
using Restaurant.Application.Repositories;
using RestaurantService.Domain.Entities;

namespace Restaurant.Application.Services.impl
{
    public class AdminMarketingService : IAdminMarketingService
    {
        private readonly IRestaurantManagementRepository _repository;
        private readonly ILogger<AdminMarketingService> _logger;

        public AdminMarketingService(
            IRestaurantManagementRepository repository,
            ILogger<AdminMarketingService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<BaseResponse<AdminMarketingDto>> GetMarketingAsync(
            Guid restaurantId, CancellationToken ct = default)
        {
            try
            {
                var marketing = await _repository.GetMarketingAsync(restaurantId, ct);
                if (marketing == null)
                    return BaseResponse<AdminMarketingDto>.Failure("Marketing info not found for this restaurant.");

                return BaseResponse<AdminMarketingDto>.Success(ToDto(marketing));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting marketing for restaurant {RestaurantId}", restaurantId);
                return BaseResponse<AdminMarketingDto>.Failure($"Error: {ex.Message}");
            }
        }

        public async Task<BaseResponse<AdminMarketingDto>> UpdateMarketingAsync(
            Guid restaurantId, UpdateRestaurantMarketingRequest request, CancellationToken ct = default)
        {
            try
            {
                var existing = await _repository.GetMarketingAsync(restaurantId, ct);

                var marketing = existing ?? new RestaurantMarketing { RestaurantId = restaurantId };

                if (request.AllowsPromotions.HasValue) marketing.AllowsPromotions = request.AllowsPromotions.Value;
                if (request.IsFeatured.HasValue) marketing.IsFeatured = request.IsFeatured.Value;
                if (request.SearchPriority.HasValue) marketing.SearchPriority = request.SearchPriority.Value;
                if (request.AccountManagerName != null) marketing.AccountManagerName = request.AccountManagerName.Trim();
                if (request.AccountManagerEmail != null) marketing.AccountManagerEmail = request.AccountManagerEmail.Trim();

                await _repository.UpsertMarketingAsync(marketing, ct);

                _logger.LogInformation(
                    "Admin updated marketing for restaurant {RestaurantId}: Featured={IsFeatured}, Priority={Priority}",
                    restaurantId, marketing.IsFeatured, marketing.SearchPriority);

                var updated = await _repository.GetMarketingAsync(restaurantId, ct);
                return BaseResponse<AdminMarketingDto>.Success(ToDto(updated!));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating marketing for restaurant {RestaurantId}", restaurantId);
                return BaseResponse<AdminMarketingDto>.Failure($"Error: {ex.Message}");
            }
        }

        public async Task<BaseResponse<List<AdminMarketingDto>>> GetAllFeaturedAsync(CancellationToken ct = default)
        {
            try
            {
                var featured = await _repository.GetAllFeaturedOrderByPriorityAsync(ct);

                return BaseResponse<List<AdminMarketingDto>>.Success(featured.Select(ToDto).ToList());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all featured restaurants");
                return BaseResponse<List<AdminMarketingDto>>.Failure($"Error: {ex.Message}");
            }
        }

        private static AdminMarketingDto ToDto(RestaurantMarketing m) => new()
        {
            Id = m.Id,
            RestaurantId = m.RestaurantId,
            RestaurantName = m.Restaurant?.Name ?? string.Empty,
            AllowsPromotions = m.AllowsPromotions,
            IsFeatured = m.IsFeatured,
            SearchPriority = m.SearchPriority,
            AccountManagerName = m.AccountManagerName,
            AccountManagerEmail = m.AccountManagerEmail
        };
    }
}
