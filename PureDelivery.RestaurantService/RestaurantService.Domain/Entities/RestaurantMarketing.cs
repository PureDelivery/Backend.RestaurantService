using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantService.Domain.Entities
{
    public class RestaurantMarketing
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid RestaurantId { get; set; }

        // Маркетинг
        public bool AllowsPromotions { get; set; } = true;
        public bool IsFeatured { get; set; }
        public int SearchPriority { get; set; } = 1;

        // Менеджер (временно здесь, потом вынесем)
        public string AccountManagerName { get; set; } = string.Empty;
        public string AccountManagerEmail { get; set; } = string.Empty;

        // Navigation Property
        public virtual Restaurant Restaurant { get; set; } = null!;
    }
}
