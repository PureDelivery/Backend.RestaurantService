using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantService.Domain.Entities
{
    public class RestaurantSettings
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid RestaurantId { get; set; }

        // Заказы
        public decimal MinOrderAmount { get; set; }
        public bool AcceptPreOrders { get; set; } = true;
        public int MaxPreOrderDays { get; set; } = 7;

        // Время приготовления
        public int AveragePreparationMinutes { get; set; } = 20;
        public int MaxPreparationMinutes { get; set; } = 45;

        // Контакты
        public string Phone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Website { get; set; } = string.Empty;

        public virtual Restaurant Restaurant { get; set; } = null!;
    }

}
