using PureDelivery.Shared.Contracts.DTOs.Location.Requests;
using PureDelivery.Shared.Contracts.DTOs.Restaurants.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestaurantDomain = RestaurantService.Domain.Entities.Restaurant;

namespace Restaurant.Application.Mappers
{
    public interface ILocationRequestMapper
    {
        FilterRestaurantsByLocationRequest MapToFilterRequest(
            IEnumerable<RestaurantDomain> restaurants,
            LocationRequestBase request);

        RestaurantLocationData MapToLocationData(RestaurantDomain restaurant);
    }
}
