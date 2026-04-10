using Microsoft.Extensions.Logging;
using PureDelivery.Shared.Contracts.Domain.Models;
using Restaurant.Application.DTOs.RestaurantManagement;
using Restaurant.Application.Repositories;
using RestaurantService.Domain.Entities;
using RestaurantService.Domain.Enums;
using RestaurantDomain = RestaurantService.Domain.Entities.Restaurant;

namespace Restaurant.Application.Services.impl
{
    public class RestaurantManagementService : IRestaurantManagementService
    {
        private readonly IRestaurantManagementRepository _repository;
        private readonly ILogger<RestaurantManagementService> _logger;

        public RestaurantManagementService(
            IRestaurantManagementRepository repository,
            ILogger<RestaurantManagementService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<BaseResponse<RestaurantManagementDetailDto>> GetRestaurantAsync(
            Guid restaurantId, CancellationToken ct = default)
        {
            try
            {
                var restaurant = await _repository.GetForManagementAsync(restaurantId, ct);
                if (restaurant == null)
                    return BaseResponse<RestaurantManagementDetailDto>.Failure("Restaurant not found.");

                var workingHours = await _repository.GetWorkingHoursAsync(restaurantId, ct);
                return BaseResponse<RestaurantManagementDetailDto>.Success(ToDto(restaurant, workingHours));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting restaurant {RestaurantId}", restaurantId);
                return BaseResponse<RestaurantManagementDetailDto>.Failure($"Error: {ex.Message}");
            }
        }

        public async Task<BaseResponse<RestaurantManagementDetailDto>> UpdateInfoAsync(
            Guid restaurantId, UpdateRestaurantInfoRequest request, CancellationToken ct = default)
        {
            try
            {
                var restaurant = await _repository.GetForManagementAsync(restaurantId, ct);
                if (restaurant == null)
                    return BaseResponse<RestaurantManagementDetailDto>.Failure("Restaurant not found.");

                if (request.Name != null) restaurant.Name = request.Name.Trim();
                if (request.Description != null) restaurant.Description = request.Description.Trim();
                if (request.ImageUrl != null) restaurant.ImageUrl = request.ImageUrl.Trim();
                if (request.CuisineTypes.HasValue) restaurant.CuisineTypes = request.CuisineTypes.Value;
                if (request.Tags.HasValue) restaurant.Tags = request.Tags.Value;

                await _repository.UpdateRestaurantAsync(restaurant, ct);

                var workingHours = await _repository.GetWorkingHoursAsync(restaurantId, ct);
                return BaseResponse<RestaurantManagementDetailDto>.Success(ToDto(restaurant, workingHours));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating restaurant info {RestaurantId}", restaurantId);
                return BaseResponse<RestaurantManagementDetailDto>.Failure($"Error: {ex.Message}");
            }
        }

        public async Task<BaseResponse<RestaurantSettingsManagementDto>> UpdateSettingsAsync(
            Guid restaurantId, UpdateRestaurantSettingsRequest request, CancellationToken ct = default)
        {
            try
            {
                var restaurant = await _repository.GetForManagementAsync(restaurantId, ct);
                if (restaurant == null)
                    return BaseResponse<RestaurantSettingsManagementDto>.Failure("Restaurant not found.");

                var settings = restaurant.Settings;
                if (settings == null)
                    return BaseResponse<RestaurantSettingsManagementDto>.Failure("Restaurant settings not found.");

                if (request.MinOrderAmount.HasValue) settings.MinOrderAmount = request.MinOrderAmount.Value;
                if (request.AcceptPreOrders.HasValue) settings.AcceptPreOrders = request.AcceptPreOrders.Value;
                if (request.MaxPreOrderDays.HasValue) settings.MaxPreOrderDays = request.MaxPreOrderDays.Value;
                if (request.AveragePreparationMinutes.HasValue) settings.AveragePreparationMinutes = request.AveragePreparationMinutes.Value;
                if (request.MaxPreparationMinutes.HasValue) settings.MaxPreparationMinutes = request.MaxPreparationMinutes.Value;
                if (request.Phone != null) settings.Phone = request.Phone.Trim();
                if (request.Email != null) settings.Email = request.Email.Trim();
                if (request.Website != null) settings.Website = request.Website.Trim();

                await _repository.UpdateSettingsAsync(settings, ct);
                return BaseResponse<RestaurantSettingsManagementDto>.Success(ToSettingsDto(settings));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating restaurant settings {RestaurantId}", restaurantId);
                return BaseResponse<RestaurantSettingsManagementDto>.Failure($"Error: {ex.Message}");
            }
        }

        public async Task<BaseResponse<List<WorkingHoursManagementDto>>> UpsertWorkingHoursAsync(
            Guid restaurantId, UpsertWorkingHoursRequest request, CancellationToken ct = default)
        {
            try
            {
                var hours = request.Hours.Select(h => new WorkingHours
                {
                    RestaurantId = restaurantId,
                    DayOfWeek = h.DayOfWeek,
                    OpenTime = h.OpenTime,
                    CloseTime = h.CloseTime,
                    IsClosed = h.IsClosed
                }).ToList();

                await _repository.UpsertWorkingHoursAsync(restaurantId, hours, ct);

                var saved = await _repository.GetWorkingHoursAsync(restaurantId, ct);
                return BaseResponse<List<WorkingHoursManagementDto>>.Success(saved.Select(ToDto).ToList());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error upserting working hours for restaurant {RestaurantId}", restaurantId);
                return BaseResponse<List<WorkingHoursManagementDto>>.Failure($"Error: {ex.Message}");
            }
        }

        // ── Mapping ───────────────────────────────────────────────────────────

        private static RestaurantManagementDetailDto ToDto(
            RestaurantDomain r, List<WorkingHours> workingHours)
        {
            var cuisines = Enum.GetValues<CuisineType>()
                .Where(v => v != CuisineType.None && r.CuisineTypes.HasFlag(v))
                .Select(v => v.ToString())
                .ToList();

            var tags = Enum.GetValues<RestaurantTag>()
                .Where(v => v != RestaurantTag.None && r.Tags.HasFlag(v))
                .Select(v => v.ToString())
                .ToList();

            return new RestaurantManagementDetailDto
            {
                Id = r.Id,
                Name = r.Name,
                Description = r.Description,
                ImageUrl = r.ImageUrl,
                IsActive = r.IsActive,
                CuisineTypes = cuisines,
                Tags = tags,
                Settings = r.Settings != null ? ToSettingsDto(r.Settings) : new(),
                WorkingHours = workingHours.Select(ToDto).ToList(),
                CreatedAt = r.CreatedAt,
                UpdatedAt = r.UpdatedAt
            };
        }

        private static RestaurantSettingsManagementDto ToSettingsDto(RestaurantSettings s) => new()
        {
            MinOrderAmount = s.MinOrderAmount,
            AcceptPreOrders = s.AcceptPreOrders,
            MaxPreOrderDays = s.MaxPreOrderDays,
            AveragePreparationMinutes = s.AveragePreparationMinutes,
            MaxPreparationMinutes = s.MaxPreparationMinutes,
            Phone = s.Phone,
            Email = s.Email,
            Website = s.Website
        };

        private static WorkingHoursManagementDto ToDto(WorkingHours wh) => new()
        {
            Id = wh.Id,
            DayOfWeek = wh.DayOfWeek.ToString(),
            OpenTime = wh.OpenTime.ToString(@"hh\:mm"),
            CloseTime = wh.CloseTime.ToString(@"hh\:mm"),
            IsClosed = wh.IsClosed
        };
    }
}
