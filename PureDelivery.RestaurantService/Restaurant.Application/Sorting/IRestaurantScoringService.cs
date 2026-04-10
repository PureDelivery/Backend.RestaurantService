using RestaurantDomain = RestaurantService.Domain.Entities.Restaurant;

namespace Restaurant.Application.Sorting;

public interface IRestaurantScoringService
{
    decimal CalculateScore(RestaurantDomain restaurant, decimal distance = 0m);
}
