using PureDelivery.Shared.Contracts.DTOs.Location.Requests;
using PureDelivery.Shared.Contracts.DTOs.Restaurants.Requests;
using RestaurantService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestaurantDomain = RestaurantService.Domain.Entities.Restaurant;

namespace Restaurant.Application.Mappers.impl
{
    public class LocationRequestMapper : ILocationRequestMapper
    {
        public FilterRestaurantsByLocationRequest MapToFilterRequest(
            IEnumerable<RestaurantDomain> restaurants,
            LocationRequestBase request)
        {
            return new FilterRestaurantsByLocationRequest
            {
                UserLatitude = request.Latitude,
                UserLongitude = request.Longitude,
                Restaurants = restaurants.Select(MapToLocationData).ToList()
            };
        }

        public RestaurantLocationData MapToLocationData(RestaurantDomain restaurant)
        {

            return new RestaurantLocationData
            {
                RestaurantId = restaurant.Id,
                Latitude = restaurant.Address.Latitude,
                Longitude = restaurant.Address.Longitude,
                DeliveryZones = restaurant.DeliveryZones
                    .Where(z => z.IsActive)
                    .Select(MapToDeliveryZoneData)
                    .ToList()
            };
        }


        // TODO?
        private DeliveryZoneData MapToDeliveryZoneData(DeliveryZone zone)
        {
            return new DeliveryZoneData
            {
                ZoneId = zone.Id,
                Name = zone.Name,
                DeliveryFee = zone.DeliveryFee,
                MinOrderAmount = zone.MinOrderAmount,
                EstimatedDeliveryMinutes = zone.EstimatedDeliveryMinutes,
                Priority = zone.Priority,
                IsActive = zone.IsActive,
                Points = zone.Points
                    .OrderBy(p => p.Order)
                    .Select(p => new ZonePointData
                    {
                        Order = p.Order,
                        Latitude = p.Latitude,
                        Longitude = p.Longitude
                    }).ToList()
            };
        }
    }
}
