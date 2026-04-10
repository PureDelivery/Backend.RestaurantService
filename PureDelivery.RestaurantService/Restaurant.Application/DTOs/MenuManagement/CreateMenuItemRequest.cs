using RestaurantService.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Restaurant.Application.DTOs.MenuManagement
{
    public class CreateMenuItemRequest
    {
        [Required, MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string Description { get; set; } = string.Empty;

        [Required, Range(0.01, double.MaxValue)]
        public decimal Price { get; set; }

        [Required]
        public MenuCategory Category { get; set; }

        public string ImageUrl { get; set; } = string.Empty;

        [Range(0, int.MaxValue)]
        public int PreparationTimeMinutes { get; set; }

        public bool IsAvailable { get; set; } = true;
    }
}
