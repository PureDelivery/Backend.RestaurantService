using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantService.Domain.Entities
{
    public class MenuItemPopularity
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public bool IsPopular { get; set; }
        public bool IsRecommended { get; set; }
        public bool IsNew { get; set; }

        public Guid MenuItemId { get; set; }
        public virtual MenuItem MenuItem { get; set; } = null!;

    }
}
