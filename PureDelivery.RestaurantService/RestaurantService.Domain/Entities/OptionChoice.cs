using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantService.Domain.Entities
{
    public class OptionChoice
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid OptionId { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal PriceModifier { get; set; } = 0;
        public bool IsDefault { get; set; } = false;
        public bool IsAvailable { get; set; } = true;
        public int SortOrder { get; set; } = 0;

        // Navigation Property
        public virtual MenuItemOption Option { get; set; } = null!;
    }
}
