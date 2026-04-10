using PureDelivery.Shared.Contracts.DTOs.Location.Responses;
using PureDelivery.Shared.Contracts.DTOs.Restaurants.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestaurantDomain = RestaurantService.Domain.Entities.Restaurant;

namespace Restaurant.Application.Services.External
{
    public interface ILocationServiceClient
    {
        Task<FilterRestaurantsByLocationResponse> CallGetRestaurantsAvailableForLocation(
            IEnumerable<RestaurantDomain> restaurants,
            LocationRequestBase request);

        Task<FilterRestaurantsByLocationResponse> CallGetRestaurantAvailableForLocation(
            RestaurantDomain restaurant,
            LocationRequestBase request);
    }
}
