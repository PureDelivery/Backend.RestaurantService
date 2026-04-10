using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantService.Domain.Configuration
{
    public class ScoringSettings
    {
        public decimal SearchPriorityWeight { get; set; }
        public decimal DeliveryZonePriorityWeight { get; set; }
        public decimal DistanceWeight { get; set; }
    }
}
