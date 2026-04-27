using Microsoft.Extensions.Logging;
using PureDelivery.Common.Configuration.Services;
using PureDelivery.Common.Http.Models;
using PureDelivery.Common.Http.Services;
using PureDelivery.Shared.Contracts.Domain.Models;
using PureDelivery.Shared.Contracts.DTOs.Location.Requests;
using PureDelivery.Shared.Contracts.DTOs.Location.Responses;
using PureDelivery.Shared.Contracts.DTOs.Restaurants.Requests;
using PureDelivery.Shared.Contracts.DTOs.Restaurants.Responses;
using Restaurant.Application.Exceptions;
using Restaurant.Application.Mappers;
using Restaurant.Application.Pagination;
using Restaurant.Application.Repositories;
using Restaurant.Application.Services.External;
using Restaurant.Application.Sorting;
using RestaurantService.Domain.Entities;
using RestaurantDomain = RestaurantService.Domain.Entities.Restaurant;

namespace Restaurant.Application.Services.impl;

public class RestaurantService : IRestaurantService
{
    private readonly IRestaurantRepository _restaurantRepository;
    private readonly ILocationServiceClient _locationServiceClient;
    private readonly ILocationResponseMapper _locationResponseMapper;
    private readonly IRestaurantSortingService _sortingService;
    private readonly IRestaurantMapper _mapper;
    private readonly IPaginationService _paginationService;
    private readonly ILogger<RestaurantService> _logger;

    public RestaurantService(
        IRestaurantRepository restaurantRepository,
        ILocationServiceClient locationServiceClient,
        ILocationResponseMapper locationResponseMapper,
        IRestaurantSortingService sortingService,
        IRestaurantMapper mapper,
        IPaginationService paginationService,
        ILogger<RestaurantService> logger)
    {
        _restaurantRepository = restaurantRepository;
        _locationServiceClient = locationServiceClient;
        _locationResponseMapper = locationResponseMapper;
        _sortingService = sortingService;
        _mapper = mapper;
        _paginationService = paginationService;
        _logger = logger;
    }




    // ADD LOGGING EVERYWHERE

    // ADD PATTERN??? TEMPLATE METHOD??? FOR SAME FLOW SEARCHING AND FILTERING





    public async Task<PagedResult<RestaurantDto>> GetRestaurantsByLocationAsync(GetRestaurantsRequest request)
    {
        try
        {
            _logger.LogInformation("Getting restaurants for location: ({Lat}, {Lng})",
                    request.Latitude, request.Longitude);

            // 1. Получаем ВСЕ рестораны
            var allRestaurants = await _restaurantRepository.GetRestaurantsDataAsync();

            if (!allRestaurants.Any())
            {
                _logger.LogWarning("No active restaurants found in database");
                return _paginationService.CreatePagedResult(
                    new List<RestaurantDto>(),
                    request.Page,
                    request.PageSize);
            }

            // 2. Узнаём у Location Service - какие доставляются + доп.данные
            var locationResponse = await _locationServiceClient.CallGetRestaurantsAvailableForLocation(allRestaurants, request);

            // 3. Фильтруем наши рестораны по результатам Location Service
            var deliverableIds = locationResponse.DeliverableRestaurants.Select(dr => dr.RestaurantId).ToHashSet();
            var filteredRestaurants = allRestaurants.Where(r => deliverableIds.Contains(r.Id));

            // 4. Сортируем доменные модели
            var sortedRestaurants = _sortingService.SortByRelevance(filteredRestaurants);

            // 5. Маппим, передавая и рестораны, и location response
            var restaurantDtos = _mapper.MapToDtos(sortedRestaurants, locationResponse);

            _logger.LogInformation("Found {Count} deliverable restaurants", restaurantDtos.Count);

            return _paginationService.CreatePagedResult(restaurantDtos, request.Page, request.PageSize);
        }
        catch (RestaurantException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while getting restaurants by location");
            throw RestaurantException.InternalError(
                $"Unexpected error in GetRestaurantsByLocationAsync: {ex.Message}");
        }
    }


    public async Task<RestaurantDetailDto> GetRestaurantDetailAsync(Guid restaurantId, GetRestaurantsRequest request)
    {
        try
        {
            _logger.LogInformation("Getting restaurant detail for: {RestaurantId}", restaurantId);

            var restaurant = await _restaurantRepository.GetRestaurantByIdAsync(restaurantId);

            if (restaurant == null)
            {
                throw RestaurantException.NotFound(restaurantId);
            }

            var locationResponse = await _locationServiceClient
                .CallGetRestaurantAvailableForLocation(restaurant, request);

            var isDeliverable = locationResponse.DeliverableRestaurants
                .Any(dr => dr.RestaurantId == restaurantId);

            if (!isDeliverable)
            {
                _logger.LogWarning("Restaurant {RestaurantId} doesn't deliver to location ({Lat}, {Lng})",
                    restaurantId, request.Latitude, request.Longitude);
                throw RestaurantException.NotDeliverable();
            }

            var result = _mapper.MapToDetailDto(restaurant, locationResponse);
            _logger.LogInformation("Successfully retrieved restaurant detail for {RestaurantId}", restaurantId);

            return result;
        }
        catch (RestaurantException)
        {
            throw; // Перебрасываем наши исключения
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error getting restaurant detail for {RestaurantId}", restaurantId);
            throw RestaurantException.InternalError(
                $"Unexpected error in GetRestaurantDetailAsync: {ex.Message}");
        }
    }


    public async Task<PagedResult<RestaurantDto>> SearchRestaurantsAsync(SearchRestaurantRequest request)
    {
        try
        {
            _logger.LogInformation("Searching restaurants for query: '{Query}'", request.Query);

            var foundRestaurants = await _restaurantRepository.SearchRestaurantsByNameAsync(request.Query);

            if (!foundRestaurants.Any())
            {
                _logger.LogInformation("No restaurants found for query: '{Query}'", request.Query);
                return _paginationService.CreatePagedResult(
                    new List<RestaurantDto>(),
                    request.Page,
                    request.PageSize);
            }

            var locationResponse = await _locationServiceClient.CallGetRestaurantsAvailableForLocation(foundRestaurants, request);

            var sortedRestaurants = _sortingService.SortByRelevance(foundRestaurants);
            var restaurantDtos = _mapper.MapToDtos(sortedRestaurants, locationResponse);

            _logger.LogInformation("Found {Count} restaurants for query: '{Query}'", restaurantDtos.Count, request.Query);
            return _paginationService.CreatePagedResult(restaurantDtos, request.Page, request.PageSize);
        }
        catch (RestaurantException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching restaurants for query: '{Query}'", request.Query);
            throw RestaurantException.InternalError(
                $"Unexpected error in SearchRestaurantsAsync: {ex.Message}");
        }
    }

    public async Task<PagedResult<RestaurantDto>> SearchRestaurantsByDishAsync(SearchRestaurantByDishRequest request)
    {
        try
        {
            _logger.LogInformation("Searching restaurants by dish: '{Query}'", request.Query);

            var restaurantsWithDish = await _restaurantRepository.SearchRestaurantsByDishNameAsync(request.Query);

            if (!restaurantsWithDish.Any())
            {
                _logger.LogInformation("No restaurants found with dish: '{Query}'", request.Query);
                return _paginationService.CreatePagedResult(
                    new List<RestaurantDto>(),
                    request.Page,
                    request.PageSize);
            }

            var locationResponse = await _locationServiceClient.CallGetRestaurantsAvailableForLocation(restaurantsWithDish, request);
            var sortedRestaurants = _sortingService.SortByRelevance(restaurantsWithDish);
            var restaurantDtos = _mapper.MapToDtos(sortedRestaurants, locationResponse);

            _logger.LogInformation("Found {Count} restaurants with dish: '{Query}'", restaurantDtos.Count, request.Query);
            return _paginationService.CreatePagedResult(restaurantDtos, request.Page, request.PageSize);
        }
        catch (RestaurantException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching restaurants by dish: '{Query}'", request.Query);
            throw RestaurantException.InternalError(
                $"Unexpected error in SearchRestaurantsByDishAsync: {ex.Message}");
        }
    }

    public async Task<PagedResult<RestaurantDto>> FilterRestaurantsAsync(FilterRestaurantsRequest request)
    {
        try
        {
            _logger.LogInformation("Filtering restaurants - Cuisines: {CuisineTypes}, Tags: {Tags}",
                request.CuisineTypes?.Any() == true ? string.Join(", ", request.CuisineTypes) : "none",
                request.Tags?.Any() == true ? string.Join(", ", request.Tags) : "none");

            if (request.Latitude == 0 || request.Longitude == 0)
            {
                throw RestaurantException.ValidationError(
                    "Location coordinates are required");
            }

            if ((request.CuisineTypes?.Any() != true) && (request.Tags?.Any() != true))
            {
                throw RestaurantException.ValidationError(
                    "Please select at least one cuisine type or tag");
            }

            var filteredRestaurants = await _restaurantRepository.FilterRestaurantsAsync(
                request.CuisineTypes, request.Tags);

            if (!filteredRestaurants.Any())
            {
                _logger.LogInformation("No restaurants found matching filter criteria");
                return _paginationService.CreatePagedResult(
                    new List<RestaurantDto>(),
                    request.Page,
                    request.PageSize);
            }

            var locationResponse = await _locationServiceClient.CallGetRestaurantsAvailableForLocation(filteredRestaurants, request);
            var sortedRestaurants = _sortingService.SortByRelevance(filteredRestaurants);
            var restaurantDtos = _mapper.MapToDtos(sortedRestaurants, locationResponse);

            _logger.LogInformation("Found {Count} restaurants matching filter criteria", restaurantDtos.Count);
            return _paginationService.CreatePagedResult(restaurantDtos, request.Page, request.PageSize);
        }
        catch (RestaurantException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error in restaurant filtering");
            throw RestaurantException.InternalError(
                $"Unexpected error in FilterRestaurantsAsync: {ex.Message}");
        }
    }

    public async Task<RestaurantDetailDto> GetRestaurantByIdAsync(Guid restaurantId)
    {
        var restaurant = await _restaurantRepository.GetRestaurantByIdAsync(restaurantId);

        if (restaurant == null)
        {
            throw RestaurantException.NotFound(restaurantId);
        }

        return _mapper.MapToDetailDto(restaurant);
    }

    public async Task<List<Guid>> GetRestaurantIdsByLocationAsync(double latitude, double longitude)
    {
        try
        {
            var allRestaurants = await _restaurantRepository.GetRestaurantsDataAsync();

            if (!allRestaurants.Any())
                return [];

            var fakeRequest = new GetRestaurantsRequest { Latitude = latitude, Longitude = longitude, Page = 1, PageSize = int.MaxValue };
            var locationResponse = await _locationServiceClient.CallGetRestaurantsAvailableForLocation(allRestaurants, fakeRequest);

            return locationResponse.DeliverableRestaurants.Select(dr => dr.RestaurantId).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting restaurant IDs by location ({Lat}, {Lng})", latitude, longitude);
            return [];
        }
    }
}