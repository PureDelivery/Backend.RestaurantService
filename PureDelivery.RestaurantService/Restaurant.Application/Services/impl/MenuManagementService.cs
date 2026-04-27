using MassTransit;
using Microsoft.Extensions.Logging;
using PureDelivery.Shared.Contracts.Domain.Models;
using PureDelivery.Shared.Contracts.Events.Restaurant;
using Restaurant.Application.DTOs.MenuManagement;
using Restaurant.Application.Repositories;
using RestaurantService.Domain.Entities;

namespace Restaurant.Application.Services.impl
{
    public class MenuManagementService : IMenuManagementService
    {
        private readonly IMenuRepository _menuRepository;
        private readonly ILogger<MenuManagementService> _logger;
        private readonly IPublishEndpoint _publishEndpoint;
        public MenuManagementService(IMenuRepository menuRepository, ILogger<MenuManagementService> logger, IPublishEndpoint publishEndpoint)
        {
            _menuRepository = menuRepository;
            _logger = logger;
            _publishEndpoint = publishEndpoint;
        }

        // ── MenuItem ──────────────────────────────────────────────────────────

        public async Task<BaseResponse<List<MenuItemManagementDto>>> GetAllItemsAsync(
            Guid restaurantId, CancellationToken ct = default)
        {
            try
            {
                var items = await _menuRepository.GetAllMenuItemsForManagementAsync(restaurantId, ct);
                return BaseResponse<List<MenuItemManagementDto>>.Success(items.Select(ToDto).ToList());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting menu items for restaurant {RestaurantId}", restaurantId);
                return BaseResponse<List<MenuItemManagementDto>>.Failure($"Error: {ex.Message}");
            }
        }

        public async Task<BaseResponse<MenuItemManagementDto>> GetItemAsync(
            Guid restaurantId, Guid itemId, CancellationToken ct = default)
        {
            try
            {
                var item = await _menuRepository.GetMenuItemRawAsync(restaurantId, itemId, ct);
                if (item == null) return BaseResponse<MenuItemManagementDto>.Failure("Menu item not found.");
                return BaseResponse<MenuItemManagementDto>.Success(ToDto(item));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting menu item {ItemId}", itemId);
                return BaseResponse<MenuItemManagementDto>.Failure($"Error: {ex.Message}");
            }
        }

        public async Task<BaseResponse<MenuItemManagementDto>> CreateItemAsync(
            Guid restaurantId, CreateMenuItemRequest request, CancellationToken ct = default)
        {
            try
            {
                var item = new MenuItem
                {
                    RestaurantId = restaurantId,
                    Name = request.Name.Trim(),
                    Description = request.Description.Trim(),
                    Price = request.Price,
                    Category = request.Category,
                    ImageUrl = request.ImageUrl.Trim(),
                    PreparationTimeMinutes = request.PreparationTimeMinutes,
                    IsAvailable = request.IsAvailable,
                    Nutrition = new NutritionInfo
                    {
                        CaloriesPer100g = request.Nutrition.CaloriesPer100g,
                        ProteinPer100g = request.Nutrition.ProteinPer100g,
                        FatPer100g = request.Nutrition.FatPer100g,
                        CarbsPer100g = request.Nutrition.CarbsPer100g,
                        WeightGrams = request.Nutrition.WeightGrams,
                        Allergens = request.Nutrition.Allergens,
                        DietaryTags = request.Nutrition.DietaryTags,
                    }
                };

                await _menuRepository.AddMenuItemAsync(item, ct);
                _logger.LogInformation("Created menu item {ItemId} for restaurant {RestaurantId}", item.Id, restaurantId);

                await _publishEndpoint.Publish(new NewMenuItemAddedEvent
                {
                    MenuItemId = item.Id,
                    RestaurantId = restaurantId,
                    Name = item.Name,
                    Description = item.Description,
                    Category = (int)item.Category,
                    Price = item.Price,
                    CaloriesPer100g = item.Nutrition.CaloriesPer100g,
                    ProteinPer100g = item.Nutrition.ProteinPer100g,
                    FatPer100g = item.Nutrition.FatPer100g,
                    CarbsPer100g = item.Nutrition.CarbsPer100g,
                    WeightGrams = item.Nutrition.WeightGrams,
                    Allergens = (int)item.Nutrition.Allergens,
                    DietaryTags = (int)item.Nutrition.DietaryTags,
                    IsPopular = false,
                    IsRecommended = false,
                    ImageUrl = item.ImageUrl ?? string.Empty,
                }, ct);

                return BaseResponse<MenuItemManagementDto>.Success(ToDto(item));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating menu item for restaurant {RestaurantId}", restaurantId);
                return BaseResponse<MenuItemManagementDto>.Failure($"Error: {ex.Message}");
            }
        }

        public async Task<BaseResponse<MenuItemManagementDto>> UpdateItemAsync(
            Guid restaurantId, Guid itemId, UpdateMenuItemRequest request, CancellationToken ct = default)
        {
            try
            {
                var item = await _menuRepository.GetMenuItemRawAsync(restaurantId, itemId, ct);
                if (item == null) return BaseResponse<MenuItemManagementDto>.Failure("Menu item not found.");

                if (request.Name != null) item.Name = request.Name.Trim();
                if (request.Description != null) item.Description = request.Description.Trim();
                if (request.Price.HasValue) item.Price = request.Price.Value;
                if (request.Category.HasValue) item.Category = request.Category.Value;
                if (request.ImageUrl != null) item.ImageUrl = request.ImageUrl.Trim();
                if (request.PreparationTimeMinutes.HasValue) item.PreparationTimeMinutes = request.PreparationTimeMinutes.Value;
                if (request.IsAvailable.HasValue) item.IsAvailable = request.IsAvailable.Value;
                if (request.UnavailabilityReason != null) item.UnavailabilityReason = request.UnavailabilityReason;

                await _menuRepository.UpdateMenuItemAsync(item, ct);
                return BaseResponse<MenuItemManagementDto>.Success(ToDto(item));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating menu item {ItemId}", itemId);
                return BaseResponse<MenuItemManagementDto>.Failure($"Error: {ex.Message}");
            }
        }

        public async Task<BaseResponse<bool>> DeleteItemAsync(
            Guid restaurantId, Guid itemId, CancellationToken ct = default)
        {
            try
            {
                var item = await _menuRepository.GetMenuItemRawAsync(restaurantId, itemId, ct);
                if (item == null) return BaseResponse<bool>.Failure("Menu item not found.");

                await _menuRepository.DeleteMenuItemAsync(item, ct);
                return BaseResponse<bool>.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting menu item {ItemId}", itemId);
                return BaseResponse<bool>.Failure($"Error: {ex.Message}");
            }
        }

        // ── Option ────────────────────────────────────────────────────────────

        public async Task<BaseResponse<OptionManagementDto>> AddOptionAsync(
            Guid restaurantId, Guid itemId, CreateMenuItemOptionRequest request, CancellationToken ct = default)
        {
            try
            {
                var item = await _menuRepository.GetMenuItemRawAsync(restaurantId, itemId, ct);
                if (item == null) return BaseResponse<OptionManagementDto>.Failure("Menu item not found.");

                var option = new MenuItemOption
                {
                    MenuItemId = itemId,
                    Name = request.Name.Trim(),
                    Type = request.Type,
                    IsRequired = request.IsRequired,
                    SortOrder = request.SortOrder
                };

                await _menuRepository.AddOptionAsync(option, ct);
                return BaseResponse<OptionManagementDto>.Success(ToDto(option));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding option to item {ItemId}", itemId);
                return BaseResponse<OptionManagementDto>.Failure($"Error: {ex.Message}");
            }
        }

        public async Task<BaseResponse<OptionManagementDto>> UpdateOptionAsync(
            Guid restaurantId, Guid itemId, Guid optionId, UpdateMenuItemOptionRequest request, CancellationToken ct = default)
        {
            try
            {
                var item = await _menuRepository.GetMenuItemRawAsync(restaurantId, itemId, ct);
                if (item == null) return BaseResponse<OptionManagementDto>.Failure("Menu item not found.");

                var option = await _menuRepository.GetOptionAsync(itemId, optionId, ct);
                if (option == null) return BaseResponse<OptionManagementDto>.Failure("Option not found.");

                if (request.Name != null) option.Name = request.Name.Trim();
                if (request.Type.HasValue) option.Type = request.Type.Value;
                if (request.IsRequired.HasValue) option.IsRequired = request.IsRequired.Value;
                if (request.SortOrder.HasValue) option.SortOrder = request.SortOrder.Value;

                await _menuRepository.UpdateOptionAsync(option, ct);
                return BaseResponse<OptionManagementDto>.Success(ToDto(option));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating option {OptionId}", optionId);
                return BaseResponse<OptionManagementDto>.Failure($"Error: {ex.Message}");
            }
        }

        public async Task<BaseResponse<bool>> DeleteOptionAsync(
            Guid restaurantId, Guid itemId, Guid optionId, CancellationToken ct = default)
        {
            try
            {
                var item = await _menuRepository.GetMenuItemRawAsync(restaurantId, itemId, ct);
                if (item == null) return BaseResponse<bool>.Failure("Menu item not found.");

                var option = await _menuRepository.GetOptionAsync(itemId, optionId, ct);
                if (option == null) return BaseResponse<bool>.Failure("Option not found.");

                await _menuRepository.DeleteOptionAsync(option, ct);
                return BaseResponse<bool>.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting option {OptionId}", optionId);
                return BaseResponse<bool>.Failure($"Error: {ex.Message}");
            }
        }

        // ── Choice ────────────────────────────────────────────────────────────

        public async Task<BaseResponse<ChoiceManagementDto>> AddChoiceAsync(
            Guid restaurantId, Guid itemId, Guid optionId, CreateOptionChoiceRequest request, CancellationToken ct = default)
        {
            try
            {
                var item = await _menuRepository.GetMenuItemRawAsync(restaurantId, itemId, ct);
                if (item == null) return BaseResponse<ChoiceManagementDto>.Failure("Menu item not found.");

                var option = await _menuRepository.GetOptionAsync(itemId, optionId, ct);
                if (option == null) return BaseResponse<ChoiceManagementDto>.Failure("Option not found.");

                var choice = new OptionChoice
                {
                    OptionId = optionId,
                    Name = request.Name.Trim(),
                    PriceModifier = request.PriceModifier,
                    IsDefault = request.IsDefault,
                    IsAvailable = request.IsAvailable,
                    SortOrder = request.SortOrder
                };

                await _menuRepository.AddChoiceAsync(choice, ct);
                return BaseResponse<ChoiceManagementDto>.Success(ToDto(choice));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding choice to option {OptionId}", optionId);
                return BaseResponse<ChoiceManagementDto>.Failure($"Error: {ex.Message}");
            }
        }

        public async Task<BaseResponse<ChoiceManagementDto>> UpdateChoiceAsync(
            Guid restaurantId, Guid itemId, Guid optionId, Guid choiceId, UpdateOptionChoiceRequest request, CancellationToken ct = default)
        {
            try
            {
                var item = await _menuRepository.GetMenuItemRawAsync(restaurantId, itemId, ct);
                if (item == null) return BaseResponse<ChoiceManagementDto>.Failure("Menu item not found.");

                var option = await _menuRepository.GetOptionAsync(itemId, optionId, ct);
                if (option == null) return BaseResponse<ChoiceManagementDto>.Failure("Option not found.");

                var choice = await _menuRepository.GetChoiceAsync(optionId, choiceId, ct);
                if (choice == null) return BaseResponse<ChoiceManagementDto>.Failure("Choice not found.");

                if (request.Name != null) choice.Name = request.Name.Trim();
                if (request.PriceModifier.HasValue) choice.PriceModifier = request.PriceModifier.Value;
                if (request.IsDefault.HasValue) choice.IsDefault = request.IsDefault.Value;
                if (request.IsAvailable.HasValue) choice.IsAvailable = request.IsAvailable.Value;
                if (request.SortOrder.HasValue) choice.SortOrder = request.SortOrder.Value;

                await _menuRepository.UpdateChoiceAsync(choice, ct);
                return BaseResponse<ChoiceManagementDto>.Success(ToDto(choice));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating choice {ChoiceId}", choiceId);
                return BaseResponse<ChoiceManagementDto>.Failure($"Error: {ex.Message}");
            }
        }

        public async Task<BaseResponse<bool>> DeleteChoiceAsync(
            Guid restaurantId, Guid itemId, Guid optionId, Guid choiceId, CancellationToken ct = default)
        {
            try
            {
                var item = await _menuRepository.GetMenuItemRawAsync(restaurantId, itemId, ct);
                if (item == null) return BaseResponse<bool>.Failure("Menu item not found.");

                var option = await _menuRepository.GetOptionAsync(itemId, optionId, ct);
                if (option == null) return BaseResponse<bool>.Failure("Option not found.");

                var choice = await _menuRepository.GetChoiceAsync(optionId, choiceId, ct);
                if (choice == null) return BaseResponse<bool>.Failure("Choice not found.");

                await _menuRepository.DeleteChoiceAsync(choice, ct);
                return BaseResponse<bool>.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting choice {ChoiceId}", choiceId);
                return BaseResponse<bool>.Failure($"Error: {ex.Message}");
            }
        }

        // ── Mapping ───────────────────────────────────────────────────────────

        private static MenuItemManagementDto ToDto(MenuItem item) => new()
        {
            Id = item.Id,
            RestaurantId = item.RestaurantId,
            Name = item.Name,
            Description = item.Description,
            Price = item.Price,
            Category = item.Category.ToString(),
            ImageUrl = item.ImageUrl,
            IsAvailable = item.IsAvailable,
            UnavailabilityReason = item.UnavailabilityReason,
            PreparationTimeMinutes = item.PreparationTimeMinutes,
            CreatedAt = item.CreatedAt,
            UpdatedAt = item.UpdatedAt,
            Options = item.Options.Select(ToDto).ToList()
        };

        private static OptionManagementDto ToDto(MenuItemOption option) => new()
        {
            Id = option.Id,
            Name = option.Name,
            Type = option.Type.ToString(),
            IsRequired = option.IsRequired,
            SortOrder = option.SortOrder,
            Choices = option.Choices.Select(ToDto).ToList()
        };

        private static ChoiceManagementDto ToDto(OptionChoice choice) => new()
        {
            Id = choice.Id,
            Name = choice.Name,
            PriceModifier = choice.PriceModifier,
            IsDefault = choice.IsDefault,
            IsAvailable = choice.IsAvailable,
            SortOrder = choice.SortOrder
        };
    }
}
