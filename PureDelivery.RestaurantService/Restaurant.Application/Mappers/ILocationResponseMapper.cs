using PureDelivery.Shared.Contracts.DTOs.Location.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestaurantDomain = RestaurantService.Domain.Entities.Restaurant;

namespace Restaurant.Application.Mappers
{
    public interface ILocationResponseMapper
    {
        IEnumerable<RestaurantDomain> FilterRestaurantsByLocationResponse(
            IEnumerable<RestaurantDomain> originalRestaurants,
            FilterRestaurantsByLocationResponse? locationResponse);

    }
}
