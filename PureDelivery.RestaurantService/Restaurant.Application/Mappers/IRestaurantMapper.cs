using PureDelivery.Shared.Contracts.DTOs.Location.Responses;
using PureDelivery.Shared.Contracts.DTOs.Restaurants.Responses;
using RestaurantDomain = RestaurantService.Domain.Entities.Restaurant;

namespace Restaurant.Application.Mappers;

public interface IRestaurantMapper
{
    RestaurantDto MapToDto(RestaurantDomain restaurant, FilterRestaurantsByLocationResponse locationResponse);
    List<RestaurantDto> MapToDtos(IEnumerable<RestaurantDomain> restaurants,
           FilterRestaurantsByLocationResponse locationResponse);
    RestaurantDetailDto MapToDetailDto(RestaurantDomain restaurantm, FilterRestaurantsByLocationResponse locationResponse);
}