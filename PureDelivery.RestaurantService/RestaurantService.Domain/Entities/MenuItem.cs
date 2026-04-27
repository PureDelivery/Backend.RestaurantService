using PureDelivery.Shared.Contracts.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantService.Domain.Entities
{
    public class MenuItem
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid RestaurantId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        [Range(0.01, double.MaxValue, ErrorMessage = "Цена должна быть больше 0")]
        public decimal Price { get; set; }
        public MenuCategory Category { get; set; }
        public string ImageUrl { get; set; } = string.Empty;

        // Пищевая ценность и аллергены
        public NutritionInfo? Nutrition { get; set; }
        public MenuItemPopularity? Popularity { get; set; }

        // Доступность
        public bool IsAvailable { get; set; } = true;
        public string? UnavailabilityReason { get; set; }

        // Время приготовления
        [Range(0, int.MaxValue, ErrorMessage = "Время приготовления не может быть отрицательным")]
        public int PreparationTimeMinutes { get; set; }

        // Метаданные
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        public virtual Restaurant Restaurant { get; set; } = null!;
        public virtual ICollection<MenuItemOption> Options { get; set; } = new List<MenuItemOption>();
    }
}
