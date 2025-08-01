using RestaurantService.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantService.Domain.Entities
{
    public class MenuItemOption
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid MenuItemId { get; set; }
        public string Name { get; set; } = string.Empty;
        public OptionType Type { get; set; } = OptionType.Single;
        public bool IsRequired { get; set; } = false;
        public int SortOrder { get; set; } = 0;

        // Navigation Properties
        public virtual MenuItem MenuItem { get; set; } = null!;
        public virtual ICollection<OptionChoice> Choices { get; set; } = new List<OptionChoice>();
    }
}
