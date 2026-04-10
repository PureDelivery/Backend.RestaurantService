using RestaurantDomain = RestaurantService.Domain.Entities.Restaurant;

namespace Restaurant.Application.Sorting;

public interface IRestaurantSortingService
{
    IEnumerable<RestaurantDomain> SortByRelevance(IEnumerable<RestaurantDomain> restaurants);
}