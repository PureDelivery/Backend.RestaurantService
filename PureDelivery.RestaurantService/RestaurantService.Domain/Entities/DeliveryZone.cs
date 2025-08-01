using RestaurantService.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantService.Domain.Entities
{
    public class DeliveryZone
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid RestaurantId { get; set; }
        public string Name { get; set; } = string.Empty;

        [Range(0, double.MaxValue, ErrorMessage = "Стоимость доставки должна быть больше или равна 0")]
        public decimal DeliveryFee { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Минимальная сумма заказа должна быть больше или равна 0")]
        public decimal MinOrderAmount { get; set; }
        public int EstimatedDeliveryMinutes { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Приоритет должен быть больше 0")]
        public int Priority { get; set; } = 1;
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        public virtual Restaurant Restaurant { get; set; } = null!;
        public virtual ICollection<ZonePoint> Points { get; set; } = new List<ZonePoint>();
    }

    public class ZonePoint
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid ZoneId { get; set; }
        public int Order { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }

        // Navigation Property
        public virtual DeliveryZone Zone { get; set; } = null!;
    }
}
