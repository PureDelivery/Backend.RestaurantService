using Microsoft.Extensions.Options;
using PureDelivery.Common.Configuration.Services;
using PureDelivery.Common.Http.Configuration;
using PureDelivery.Shared.Contracts.DTOs.Restaurants.Responses;
using RestaurantService.Domain.Configs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant.Application.Mappers.impl
{
    public class DeliveryTimeCalculator : IDeliveryTimeCalculator
    {
        private readonly DeliveryTimeConfig _config;

        public DeliveryTimeCalculator(ICustomConfigurationProvider customConfigurationProvider)
        {
            _config = customConfigurationProvider
                .GetConfigurationAsync<DeliveryTimeConfig>("DeliveryTime")
                .GetAwaiter()
                .GetResult();
        }

        public int CalculateDeliveryTime(decimal distanceKm)
        {
            var deliveryTime = (int)Math.Ceiling((double)distanceKm * _config.MinutesPerKilometer);

            return Math.Max(_config.MinDeliveryMinutes,
                   Math.Min(deliveryTime, _config.MaxDeliveryMinutes));
        }

        public int GetDefaultDeliveryTime()
        {
            return _config.DefaultDeliveryMinutes;
        }
    }
}
