using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantService.Domain.Configs
{
    public class DeliveryTimeConfig
    {
        public double MinutesPerKilometer { get; set; } = 3.0;
        public int MinDeliveryMinutes { get; set; } = 10;
        public int MaxDeliveryMinutes { get; set; } = 45; 
        public int DefaultDeliveryMinutes { get; set; } = 25;
    }
}
