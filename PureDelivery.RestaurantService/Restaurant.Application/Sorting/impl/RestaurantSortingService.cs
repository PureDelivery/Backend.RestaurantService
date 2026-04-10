using Restaurant.Application.Sorting;
using RestaurantDomain = RestaurantService.Domain.Entities.Restaurant;

namespace Restaurant.Application.Sorting.impl;

public class RestaurantSortingService : IRestaurantSortingService
{
    private readonly IRestaurantScoringService _scoringService;

    public RestaurantSortingService(IRestaurantScoringService scoringService)
    {
        _scoringService = scoringService;
    }

    public IEnumerable<RestaurantDomain> SortByRelevance(IEnumerable<RestaurantDomain> restaurants)
    {
        return restaurants
            .Select(r => new RestaurantWithScore(r, _scoringService.CalculateScore(r)))
            .OrderBy(x => x.Score)
            .ThenBy(x => x.Restaurant.Marketing?.SearchPriority ?? int.MaxValue)
            .ThenBy(x => GetDeliveryZonePriority(x.Restaurant))
            .ThenBy(x => x.Restaurant.Name)
            .Select(x => x.Restaurant);
    }

    private int GetDeliveryZonePriority(RestaurantDomain restaurant)
    {
        return restaurant.DeliveryZones.Any()
            ? restaurant.DeliveryZones.Min(dz => dz.Priority)
            : int.MaxValue;
    }

    private record RestaurantWithScore(RestaurantDomain Restaurant, decimal Score);
}