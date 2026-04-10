using PureDelivery.Shared.Contracts.DTOs.Restaurants.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant.Application.Mappers
{
    public interface IDeliveryTimeCalculator
    {
        int CalculateDeliveryTime(decimal distanceKm);
        int GetDefaultDeliveryTime();
    }

}
