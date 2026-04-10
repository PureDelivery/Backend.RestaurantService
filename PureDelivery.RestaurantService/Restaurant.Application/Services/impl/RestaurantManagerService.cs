using Microsoft.Extensions.Logging;
using PureDelivery.Shared.Contracts.Domain.Enums;
using PureDelivery.Shared.Contracts.Domain.Models;
using PureDelivery.Shared.Contracts.DTOs.Identity.Requests;
using Restaurant.Application.DTOs;
using Restaurant.Application.Repositories;
using Restaurant.Application.Services.External;
using RestaurantService.Domain.Entities;

namespace Restaurant.Application.Services.impl
{
    public class RestaurantManagerService : IRestaurantManagerService
    {
        private readonly IRestaurantManagerRepository _repository;
        private readonly IIdentityServiceClient _identityClient;
        private readonly ILogger<RestaurantManagerService> _logger;

        public RestaurantManagerService(
            IRestaurantManagerRepository repository,
            IIdentityServiceClient identityClient,
            ILogger<RestaurantManagerService> logger)
        {
            _repository = repository;
            _identityClient = identityClient;
            _logger = logger;
        }

        public async Task<BaseResponse<ManagerDto>> CreateManagerAsync(
            CreateManagerRequest request, CancellationToken ct = default)
        {
            try
            {
                // 1. Создаём credentials в IdentityService
                var credentialResult = await _identityClient.RegisterUserCredentialAsync(
                    new RegisterUserCredentialRequest
                    {
                        Email = request.Email,
                        Password = request.Password,
                        Role = UserRole.Restaurant
                    }, ct);

                if (credentialResult == null)
                    return BaseResponse<ManagerDto>.Failure(
                        "Failed to create user credentials in IdentityService.");

                // 2. Создаём профиль менеджера локально
                var manager = new RestaurantManager
                {
                    UserId = credentialResult.UserId,
                    RestaurantId = request.RestaurantId,
                    FirstName = request.FirstName.Trim(),
                    LastName = request.LastName.Trim(),
                    Phone = request.Phone.Trim(),
                    Email = request.Email.ToLower().Trim()
                };

                await _repository.AddAsync(manager, ct);

                _logger.LogInformation("Created manager {ManagerId} (UserId: {UserId}) for restaurant {RestaurantId}",
                    manager.Id, manager.UserId, manager.RestaurantId);

                return BaseResponse<ManagerDto>.Success(manager.ToDto());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating manager for restaurant {RestaurantId}", request.RestaurantId);
                return BaseResponse<ManagerDto>.Failure($"Error creating manager: {ex.Message}");
            }
        }

        public async Task<BaseResponse<ManagerDto>> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            try
            {
                var manager = await _repository.GetByIdAsync(id, ct);
                if (manager == null)
                    return BaseResponse<ManagerDto>.Failure("Manager not found.");

                return BaseResponse<ManagerDto>.Success(manager.ToDto());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting manager {Id}", id);
                return BaseResponse<ManagerDto>.Failure($"Error: {ex.Message}");
            }
        }

        public async Task<BaseResponse<ManagerDto>> GetByUserIdAsync(Guid userId, CancellationToken ct = default)
        {
            try
            {
                var manager = await _repository.GetByUserIdAsync(userId, ct);
                if (manager == null)
                    return BaseResponse<ManagerDto>.Failure("Manager not found.");

                return BaseResponse<ManagerDto>.Success(manager.ToDto());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting manager by UserId {UserId}", userId);
                return BaseResponse<ManagerDto>.Failure($"Error: {ex.Message}");
            }
        }

        public async Task<BaseResponse<List<ManagerDto>>> GetByRestaurantIdAsync(
            Guid restaurantId, CancellationToken ct = default)
        {
            try
            {
                var managers = await _repository.GetByRestaurantIdAsync(restaurantId, ct);
                return BaseResponse<List<ManagerDto>>.Success(managers.Select(m => m.ToDto()).ToList());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting managers for restaurant {RestaurantId}", restaurantId);
                return BaseResponse<List<ManagerDto>>.Failure($"Error: {ex.Message}");
            }
        }

        public async Task<BaseResponse<bool>> DeactivateAsync(Guid id, CancellationToken ct = default)
        {
            try
            {
                var result = await _repository.DeactivateAsync(id, ct);
                if (!result)
                    return BaseResponse<bool>.Failure("Manager not found.");

                return BaseResponse<bool>.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deactivating manager {Id}", id);
                return BaseResponse<bool>.Failure($"Error: {ex.Message}");
            }
        }
    }

    internal static class RestaurantManagerExtensions
    {
        public static ManagerDto ToDto(this RestaurantManager m) => new()
        {
            Id = m.Id,
            UserId = m.UserId,
            RestaurantId = m.RestaurantId,
            FirstName = m.FirstName,
            LastName = m.LastName,
            Phone = m.Phone,
            Email = m.Email,
            IsActive = m.IsActive,
            CreatedAt = m.CreatedAt
        };
    }
}
