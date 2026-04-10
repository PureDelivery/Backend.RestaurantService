using PureDelivery.Shared.Contracts.DTOs.Location.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestaurantDomain = RestaurantService.Domain.Entities.Restaurant;

namespace Restaurant.Application.Mappers.impl
{
    public class LocationResponseMapper : ILocationResponseMapper
    {
        public IEnumerable<RestaurantDomain> FilterRestaurantsByLocationResponse(
            IEnumerable<RestaurantDomain> originalRestaurants,
            FilterRestaurantsByLocationResponse? locationResponse)
        {
            if (locationResponse?.DeliverableRestaurants == null || !locationResponse.DeliverableRestaurants.Any())
            {
                return Enumerable.Empty<RestaurantDomain>();
            }

            var deliverableIds = locationResponse.DeliverableRestaurants
                .Select(dr => dr.RestaurantId)
                .ToHashSet();

            return originalRestaurants
                .Where(r => deliverableIds.Contains(r.Id))
                .ToList();
        }
    }
}
