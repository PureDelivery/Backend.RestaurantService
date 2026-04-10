using Microsoft.Extensions.Logging;
using PureDelivery.Common.Configuration.Services;
using RestaurantService.Domain.Configuration;
using RestaurantDomain = RestaurantService.Domain.Entities.Restaurant;

namespace Restaurant.Application.Sorting.impl;

public class RestaurantScoringService : IRestaurantScoringService
{
    private readonly ScoringSettings _scoringSettings;

    public RestaurantScoringService(ILogger<RestaurantScoringService> logger,
                                    ICustomConfigurationProvider configProvider)
    {
        _scoringSettings = configProvider.GetConfigurationAsync<ScoringSettings>("Scoring").GetAwaiter().GetResult();
    }

    public decimal CalculateScore(RestaurantDomain restaurant, decimal distance = 0m)
    {
        var searchPriority = GetSearchPriority(restaurant);
        var deliveryZonePriority = GetDeliveryZonePriority(restaurant);

        return CalculateWeightedScore(searchPriority, deliveryZonePriority, distance);
    }

    private int GetSearchPriority(RestaurantDomain restaurant)
    {
        return restaurant.Marketing?.SearchPriority ?? int.MaxValue;
    }

    private int GetDeliveryZonePriority(RestaurantDomain restaurant)
    {
        return restaurant.DeliveryZones.Any()
            ? restaurant.DeliveryZones.Min(dz => dz.Priority)
            : int.MaxValue;
    }

    private decimal CalculateWeightedScore(int searchPriority, int deliveryZonePriority, decimal distance)
    {
        return searchPriority * _scoringSettings.SearchPriorityWeight +
               deliveryZonePriority * _scoringSettings.DeliveryZonePriorityWeight +
               distance * _scoringSettings.DistanceWeight;
    }
}
