using Microsoft.Extensions.Logging;
using PureDelivery.Common.Configuration.Services;
using PureDelivery.Common.Http.Configuration;
using PureDelivery.Common.Http.Models;
using PureDelivery.Common.Http.Services;
using PureDelivery.Shared.Contracts.Domain.Models;
using PureDelivery.Shared.Contracts.DTOs.Location.Requests;
using PureDelivery.Shared.Contracts.DTOs.Location.Responses;
using PureDelivery.Shared.Contracts.DTOs.Restaurants.Requests;
using Restaurant.Application.Builder;
using Restaurant.Application.Exceptions;
using Restaurant.Application.Mappers;
using System.Net;
using RestaurantDomain = RestaurantService.Domain.Entities.Restaurant;

namespace Restaurant.Application.Services.External.impl
{
    public class LocationServiceClient : ILocationServiceClient
    {
        private readonly IHttpApiClient _httpApiClient;
        private readonly ILocationRequestMapper _mapper;
        private readonly IHttpRequestBuilder _requestBuilder;
        private readonly HttpClientConfiguration _httpConfig;
        private readonly ILogger<LocationServiceClient> _logger;

        public LocationServiceClient(
            IHttpApiClient httpApiClient,
            ILocationRequestMapper mapper,
            IHttpRequestBuilder requestBuilder,
            ICustomConfigurationProvider configProvider,
            ILogger<LocationServiceClient> logger)
        {
            _httpApiClient = httpApiClient;
            _mapper = mapper;
            _requestBuilder = requestBuilder;
            _logger = logger;

            _httpConfig = configProvider
                .GetConfigurationAsync<HttpClientConfiguration>("HttpClient")
                .GetAwaiter()
                .GetResult();
        }

        public async Task<FilterRestaurantsByLocationResponse> CallGetRestaurantsAvailableForLocation(
            IEnumerable<RestaurantDomain> restaurants,
            LocationRequestBase request)
        {
            var restaurantsList = restaurants.ToList();
            return await CallLocationServiceInternal(
                restaurantsList,
                request,
                $"Getting location data for {restaurantsList.Count} restaurants");
        }

        public async Task<FilterRestaurantsByLocationResponse> CallGetRestaurantAvailableForLocation(
            RestaurantDomain restaurant,
            LocationRequestBase request)
        {
            return await CallLocationServiceInternal(
                new List<RestaurantDomain> { restaurant },
                request,
                $"Getting location data for restaurant {restaurant.Id}");
        }

        private async Task<FilterRestaurantsByLocationResponse> CallLocationServiceInternal(
            List<RestaurantDomain> restaurants,
            LocationRequestBase request,
            string logMessage)
        {
            try
            {
                LogRequestStart(logMessage, request);

                var filterRequest = BuildLocationRequest(restaurants, request);
                var apiResponse = await CallLocationServiceAsync(filterRequest);

                return ProcessLocationServiceResponse(apiResponse);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP error calling location service");
                throw RestaurantException.LocationServiceError("HTTP_ERROR",
                    "Location service is temporarily unavailable. Please try again later.",
                    $"HTTP error: {ex.Message}");
            }
            catch (TaskCanceledException ex)
            {
                _logger.LogError(ex, "Timeout calling location service");
                throw RestaurantException.LocationServiceError("TIMEOUT",
                    "Location service is temporarily unavailable. Please try again later.",
                    "Request timeout");
            }
            catch (RestaurantException)
            {
                throw; // Перебрасываем наши исключения
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error calling location service");
                throw RestaurantException.LocationServiceError("UNEXPECTED_ERROR",
                    "Location service is temporarily unavailable. Please try again later.",
                    $"Unexpected error: {ex.Message}");
            }
        }

        private void LogRequestStart(string logMessage, LocationRequestBase request)
        {
            if (_httpConfig.EnableDetailedLogging)
            {
                _logger.LogInformation("{LogMessage} at ({Lat}, {Lng})",
                    logMessage, request.Latitude, request.Longitude);
            }
        }

        private FilterRestaurantsByLocationRequest BuildLocationRequest(
            List<RestaurantDomain> restaurants,
            LocationRequestBase request)
        {
            var filterRequest = _mapper.MapToFilterRequest(restaurants, request);

            if (_httpConfig.LogRequestBodies && _logger.IsEnabled(LogLevel.Debug))
            {
                _logger.LogDebug("Request body: {@FilterRequest}", filterRequest);
            }

            return filterRequest;
        }

        private async Task<ApiResponse<BaseResponse<FilterRestaurantsByLocationResponse>>> CallLocationServiceAsync(
            FilterRestaurantsByLocationRequest filterRequest)
        {
            var requestParams = _requestBuilder.BuildFilterRestaurantsRequest(filterRequest);

            return await _httpApiClient.PostAsync<BaseResponse<FilterRestaurantsByLocationResponse>>(requestParams);
        }

        private FilterRestaurantsByLocationResponse ProcessLocationServiceResponse(
            ApiResponse<BaseResponse<FilterRestaurantsByLocationResponse>> apiResponse)
        {
            if (apiResponse == null)
            {
                _logger.LogError("Location service returned null response");
                throw RestaurantException.LocationServiceError("NULL_RESPONSE",
                    "Location service is temporarily unavailable. Please try again later.",
                    "Null response from service");
            }

            if (!apiResponse.IsSuccess)
            {
                _logger.LogError("Location service HTTP error: {StatusCode} - {ErrorMessage}",
                    apiResponse.StatusCode, apiResponse.ErrorMessage);

                var userMessage = apiResponse.StatusCode switch
                {
                    HttpStatusCode.BadRequest => "Invalid location request data",
                    HttpStatusCode.NotFound => "Location service endpoint not found",
                    HttpStatusCode.InternalServerError => "Location service is temporarily unavailable",
                    _ => "Location service is temporarily unavailable. Please try again later."
                };

                throw RestaurantException.LocationServiceError(
                    $"HTTP_{(int)apiResponse.StatusCode}",
                    userMessage,
                    apiResponse.ErrorMessage);
            }

            // Проверяем BaseResponse уровень
            var baseResponse = apiResponse.Data;
            if (baseResponse == null)
            {
                _logger.LogError("Location service returned null BaseResponse");
                throw RestaurantException.LocationServiceError("NULL_BASE_RESPONSE",
                    "Location service is temporarily unavailable. Please try again later.",
                    "Null BaseResponse from service");
            }

            if (!baseResponse.IsSuccess)
            {
                _logger.LogError("Location service business logic error: {Error}",
                    baseResponse.Error);

                // Анализируем ошибки бизнес-логики от Location Service
                var userMessage = AnalyzeLocationServiceError(baseResponse.Error);

                throw RestaurantException.LocationServiceError(
                    "BUSINESS_LOGIC_ERROR",
                    userMessage,
                    baseResponse.Error);
            }

            if (baseResponse.Data == null)
            {
                _logger.LogWarning("Location service returned null data in BaseResponse");
                throw RestaurantException.LocationServiceError("NULL_DATA",
                    "Location service is temporarily unavailable. Please try again later.",
                    "Empty data in BaseResponse");
            }

            LogRequestSuccess(baseResponse.Data);
            return baseResponse.Data;
        }

        private string AnalyzeLocationServiceError(string? errorMessage)
        {
            if (string.IsNullOrEmpty(errorMessage))
            {
                return "Location service is temporarily unavailable. Please try again later.";
            }

            // Анализируем типичные ошибки от Location Service
            return errorMessage.ToLower() switch
            {
                var msg when msg.Contains("restaurants") && msg.Contains("provided") =>
                    "No restaurants available for location check",
                var msg when msg.Contains("coordinates") || msg.Contains("latitude") || msg.Contains("longitude") =>
                    "Invalid location coordinates provided",
                var msg when msg.Contains("request") && msg.Contains("null") =>
                    "Invalid request data",
                var msg when msg.Contains("processing") =>
                    "Error processing location data",
                _ => "Location service is temporarily unavailable. Please try again later."
            };
        }

        private void LogRequestSuccess(FilterRestaurantsByLocationResponse data)
        {
            if (_httpConfig.EnableDetailedLogging)
            {
                _logger.LogInformation("Location service returned {DeliverableCount} deliverable restaurants",
                    data.DeliverableRestaurants?.Count ?? 0);
            }
        }
    }
}