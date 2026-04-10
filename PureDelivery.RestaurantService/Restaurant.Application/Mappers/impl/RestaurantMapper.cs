using PureDelivery.Shared.Contracts.DTOs.Location.Responses;
using PureDelivery.Shared.Contracts.DTOs.Restaurants.Responses;
using RestaurantService.Domain.Enums;
using System.ComponentModel;
using RestaurantDomain = RestaurantService.Domain.Entities.Restaurant;

namespace Restaurant.Application.Mappers.impl
{
    public class RestaurantMapper : IRestaurantMapper
    {
        private readonly IEnumConverter _enumConverter;
        private readonly IDeliveryTimeCalculator _deliveryTimeCalculator;

        public RestaurantMapper(IEnumConverter enumConverter, IDeliveryTimeCalculator deliveryTimeCalculator)
        {
            _enumConverter = enumConverter;
            _deliveryTimeCalculator = deliveryTimeCalculator;
        }

        public RestaurantDto MapToDto(
           RestaurantDomain restaurant,
           FilterRestaurantsByLocationResponse locationResponse)
        {
            var deliveryInfo = locationResponse?.DeliverableRestaurants
                .FirstOrDefault(dr => dr.RestaurantId == restaurant.Id);

            return new RestaurantDto
            {
                Id = restaurant.Id,
                Name = restaurant.Name,
                Description = restaurant.Description,
                ImageUrl = restaurant.ImageUrl,
                CuisineTypes = _enumConverter.ConvertCuisineTypes(restaurant.CuisineTypes),
                Tags = _enumConverter.ConvertTags(restaurant.Tags),
                Distance = deliveryInfo?.Distance ?? 0m,
                AverageRating = CalculateAverageRating(restaurant),
                ReviewCount = restaurant.Reviews.Count,
                EstimatedDeliveryMinutes = GetEstimatedDelivery(restaurant, deliveryInfo!),
                DeliveryFee = deliveryInfo?.BestDeliveryZone?.DeliveryFee ?? GetDeliveryFee(deliveryInfo!),
                MinOrderAmount = Math.Max(
                    restaurant.Settings.MinOrderAmount,
                    deliveryInfo?.BestDeliveryZone?.MinOrderAmount ?? 0m),
                IsFeatured = restaurant.Marketing?.IsFeatured ?? false,
                ParticipatesInLoyalty = restaurant.Partnership.ParticipatesInLoyalty,
                Latitude = restaurant.Address.Latitude,
                Longitude = restaurant.Address.Longitude
            };
        }

        public List<RestaurantDto> MapToDtos(
            IEnumerable<RestaurantDomain> restaurants,
            FilterRestaurantsByLocationResponse locationResponse)
        {
            return restaurants.Select(r => MapToDto(r, locationResponse)).ToList();
        }

        public RestaurantDetailDto MapToDetailDto(RestaurantDomain restaurant, FilterRestaurantsByLocationResponse locationResponse = null)
        {
            var deliveryInfo = locationResponse?.DeliverableRestaurants
                .FirstOrDefault(dr => dr.RestaurantId == restaurant.Id);

            if (deliveryInfo is null)
            {
                // todo
            }

            return new RestaurantDetailDto
            {
                Id = restaurant.Id,
                Name = restaurant.Name,
                Description = restaurant.Description,
                ImageUrl = restaurant.ImageUrl,
                CuisineTypes = _enumConverter.ConvertCuisineTypes(restaurant.CuisineTypes),
                Tags = _enumConverter.ConvertTags(restaurant.Tags),
                AverageRating = CalculateAverageRating(restaurant),
                ReviewCount = restaurant.Reviews.Count,
                EstimatedDelivery = GetEstimatedDelivery(restaurant, deliveryInfo!),
                DeliveryFee = GetDeliveryFee(deliveryInfo!),
                MinOrderAmount = restaurant.Settings.MinOrderAmount,
                IsOpen = IsRestaurantOpen(restaurant),
                IsFeatured = restaurant.Marketing?.IsFeatured ?? false,
                ParticipatesInLoyalty = restaurant.Partnership.ParticipatesInLoyalty,
                Address = new RestaurantAddressDto
                {
                    FullAddress = restaurant.Address.FullAddress,
                    City = restaurant.Address.City,
                    Latitude = restaurant.Address.Latitude,
                    Longitude = restaurant.Address.Longitude
                },
                Settings = new RestaurantSettingsDto
                {
                    Phone = restaurant.Settings.Phone,
                    Email = restaurant.Settings.Email,
                    Website = restaurant.Settings.Website,
                    AveragePreparationMinutes = restaurant.Settings.AveragePreparationMinutes,
                    MaxPreparationMinutes = restaurant.Settings.MaxPreparationMinutes,
                    AcceptPreOrders = restaurant.Settings.AcceptPreOrders,
                    MaxPreOrderDays = restaurant.Settings.MaxPreOrderDays
                },
                WorkingHours = restaurant.WorkingHours.Select(wh => new WorkingHoursDto
                {
                    DayOfWeek = wh.DayOfWeek,
                    OpenTime = wh.OpenTime,
                    CloseTime = wh.CloseTime,
                    IsClosed = wh.IsClosed
                })
                .OrderBy(x => (((double)x.DayOfWeek)))
                .ToList(),
                RestaurantReviews = restaurant.Reviews.Select(r => new RestaurantReviewDto
                {
                    Rating = r.Rating,
                    Comment = r.Comment
                }).ToList()
            };
        }

        private bool IsRestaurantOpen(RestaurantDomain restaurant)
        {
            var currentDateTime = DateTime.UtcNow;
            var currentDay = currentDateTime.DayOfWeek;
            var currentTime = currentDateTime.TimeOfDay;

            var todayHours = restaurant.WorkingHours
                .FirstOrDefault(wh => wh.DayOfWeek == currentDay);

            if (todayHours == null || todayHours.IsClosed)
                return false;

            if (todayHours.CloseTime < todayHours.OpenTime)
            {
                return currentTime >= todayHours.OpenTime || currentTime <= todayHours.CloseTime;
            }
            else
            {
                return currentTime >= todayHours.OpenTime && currentTime <= todayHours.CloseTime;
            }
        }

        private decimal CalculateAverageRating(RestaurantDomain restaurant)
        {
            return restaurant.Reviews.Any()
                ? (decimal)restaurant.Reviews.Average(rev => rev.Rating)
                : 0m;
        }

        private decimal GetDeliveryFee(DeliverableRestaurant deliveryInfo)
        {
            return deliveryInfo.BestDeliveryZone.DeliveryFee;
        }

        private EstimatedDelivery GetEstimatedDelivery(RestaurantDomain restaurant, DeliverableRestaurant deliveryInfo)
        {
            var baseDeliveryTime = _deliveryTimeCalculator.CalculateDeliveryTime(deliveryInfo.Distance);

            var minCookingTime = GetMinCookingTime(restaurant);
            var maxCookingTime = GetMaxCookingTime(restaurant);

            var estimatedDelivery = new EstimatedDelivery()
            {
                MinMinutes = baseDeliveryTime + minCookingTime,
                MaxMinutes = baseDeliveryTime + maxCookingTime,
            };

            return estimatedDelivery;
        }

        private int GetMinCookingTime(RestaurantDomain restaurant)
        {
            if (restaurant.MenuItems?.Any(item => item.IsAvailable) == true)
            {
                return restaurant.MenuItems
                    .Where(item => item.IsAvailable)
                    .Min(item => item.PreparationTimeMinutes);
            }

            return restaurant.Settings.AveragePreparationMinutes;
        }

        private int GetMaxCookingTime(RestaurantDomain restaurant)
        {
            if (restaurant.MenuItems?.Any(item => item.IsAvailable) == true)
            {
                return restaurant.MenuItems
                    .Where(item => item.IsAvailable)
                    .Max(item => item.PreparationTimeMinutes);
            }

            return restaurant.Settings.MaxPreparationMinutes;
        }
    }
}
